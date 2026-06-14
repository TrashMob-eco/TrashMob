#!/usr/bin/env python3
"""
Deterministic operations on Planning/Projects/*.md files.

Pulls the mechanical operations out of `.claude_common_commands.md` so they
can run without spending LLM tokens on string editing. The judgment-heavy
operations (split, merge, document a materialised risk, generate a status
report, add a code example) stay in the prose guide.

Subcommands:
    status     Set Status field on a project (table header + footer + date)
    priority   Set Priority field on a project
    complete   Mark a project complete (status + completion-date stamp)
    link       Add reciprocal `Related Documents` cross-references between
               two projects (judgment-free: just inserts links)
    risk       Append a row to the Risks & Mitigations table

Each subcommand finds the target project by its number; it scans both
`Planning/Projects/` and `Planning/Projects/Archive/`.

Usage examples:
    python scripts/project_ops.py status 60 Complete
    python scripts/project_ops.py priority 41 High
    python scripts/project_ops.py complete 60
    python scripts/project_ops.py link 41 60 \
        --x-note "Sponsored adoption pipeline feeds prospect contacts" \
        --y-note "Multi-contact tracking supports sponsorship outreach"
    python scripts/project_ops.py risk 41 \
        --title "Payment provider lock-in" --likelihood Medium \
        --impact High --mitigation "Abstract payment via Strategy pattern"
"""

from __future__ import annotations

import argparse
import datetime
import re
import sys
from pathlib import Path

PROJECTS = Path(__file__).resolve().parents[1] / "Planning" / "Projects"
ARCHIVE = PROJECTS / "Archive"


STATUS_CHOICES = [
    "Not Started", "Planning", "Ready for Review",
    "In Progress", "Developers Engaged", "Complete",
]
PRIORITY_CHOICES = ["Low", "Medium", "High", "Critical"]
LIKELIHOOD_CHOICES = ["Low", "Medium", "High"]
IMPACT_CHOICES = ["Low", "Medium", "High", "Critical"]


# ---------------------------------------------------------------------------
# File lookup
# ---------------------------------------------------------------------------

def find_project(number: int) -> Path:
    """Return the path to Project_{number}_*.md (active or archived)."""
    for root in (PROJECTS, ARCHIVE):
        matches = list(root.glob(f"Project_{number}_*.md"))
        if len(matches) == 1:
            return matches[0]
        if len(matches) > 1:
            raise FileNotFoundError(
                f"Multiple matches for project {number} in {root}: "
                + ", ".join(p.name for p in matches)
            )
    raise FileNotFoundError(
        f"No project file for number {number} in {PROJECTS} or {ARCHIVE}"
    )


def read_project(path: Path) -> str:
    """Read a project file with cp1252 fallback (legacy templates)."""
    raw = path.read_bytes()
    for enc in ("utf-8", "utf-8-sig", "cp1252"):
        try:
            return raw.decode(enc)
        except UnicodeDecodeError:
            continue
    raise UnicodeDecodeError(
        "utf-8", raw, 0, 1, f"could not decode {path}"
    )


def write_project(path: Path, content: str) -> None:
    # Write bytes (not write_text) so we preserve whatever line endings the
    # file already had. write_text with newline=None translates "\n" to the
    # platform separator, which silently corrupts CRLF-on-Windows files into
    # CRCRLF since our reader keeps \r in the buffer.
    path.write_bytes(content.encode("utf-8"))


def project_title(text: str) -> str:
    """Pull the human title out of an opening `# Project N — Title` line."""
    m = re.search(r"^#\s+Project\s+\d+\s*[—-]\s*(.+?)\s*$", text, re.MULTILINE)
    return m.group(1).strip() if m else "(unknown)"


def relative_link_to(target: Path, source: Path) -> str:
    """Return a `./...md` relative link between two project files (sibling-aware)."""
    try:
        rel = target.relative_to(source.parent).as_posix()
    except ValueError:
        # Different folders (Projects vs Archive). Walk relative.
        import os.path
        rel = os.path.relpath(target, source.parent).replace("\\", "/")
    if not rel.startswith(("./", "../")):
        rel = "./" + rel
    return rel


# ---------------------------------------------------------------------------
# Header table + footer field setters
# ---------------------------------------------------------------------------

_TODAY = datetime.date.today().isoformat()


def _set_header_field(text: str, field: str, value: str) -> tuple[str, bool]:
    """Replace `| **Field** | ... |` row; returns (new_text, did_replace).

    The trailing `\r?` tolerates CRLF line endings — Python's `$` in MULTILINE
    mode anchors before `\n` only, so files written with CRLF would otherwise
    leave a stray `\r` between the final `|` and `$` and miss the match.
    """
    pattern = rf"^\| \*\*{re.escape(field)}\*\* \| .*? \|\r?$"
    new = re.subn(
        pattern,
        f"| **{field}** | {value} |",
        text,
        count=1,
        flags=re.MULTILINE,
    )
    return new[0], (new[1] > 0)


def _set_footer_field(text: str, field: str, value: str) -> tuple[str, bool]:
    """Replace `**Field:** value` footer line."""
    pattern = rf"^\*\*{re.escape(field)}:\*\*\s+.+?\r?$"
    new = re.subn(
        pattern,
        f"**{field}:** {value}",
        text,
        count=1,
        flags=re.MULTILINE,
    )
    return new[0], (new[1] > 0)


def _bump_last_updated(text: str) -> str:
    """Set the footer's Last Updated to today; tolerant of missing field."""
    text, replaced = _set_footer_field(text, "Last Updated", _TODAY)
    if replaced:
        return text
    # Insert near the end if missing entirely.
    return text.rstrip() + f"\n\n**Last Updated:** {_TODAY}\n"


# ---------------------------------------------------------------------------
# Subcommands
# ---------------------------------------------------------------------------

def cmd_status(args: argparse.Namespace) -> int:
    path = find_project(args.number)
    text = read_project(path)
    text, ok_h = _set_header_field(text, "Status", args.status)
    text, _ = _set_footer_field(text, "Status", args.status)
    text = _bump_last_updated(text)
    if not ok_h:
        print(f"warning: no Status row found in header table of {path.name}",
              file=sys.stderr)
    write_project(path, text)
    print(f"{path.name}: Status -> {args.status}")
    return 0


def cmd_priority(args: argparse.Namespace) -> int:
    path = find_project(args.number)
    text = read_project(path)
    text, ok = _set_header_field(text, "Priority", args.priority)
    text = _bump_last_updated(text)
    if not ok:
        print(f"warning: no Priority row found in {path.name}", file=sys.stderr)
    write_project(path, text)
    print(f"{path.name}: Priority -> {args.priority}")
    return 0


def cmd_complete(args: argparse.Namespace) -> int:
    """Convenience: Status ->Complete, stamps a completion date if missing."""
    path = find_project(args.number)
    text = read_project(path)
    text, _ = _set_header_field(text, "Status", "Complete")
    text, _ = _set_footer_field(text, "Status", "Complete")
    # Append a "Completed:" footer line if not already present.
    if not re.search(r"^\*\*Completed:\*\*", text, re.MULTILINE):
        # Insert right after Last Updated, or at end.
        completed_line = f"**Completed:** {_TODAY}"
        m = re.search(r"^\*\*Last Updated:\*\*.*$", text, re.MULTILINE)
        if m:
            insert_pos = m.end()
            text = text[:insert_pos] + "\n" + completed_line + text[insert_pos:]
        else:
            text = text.rstrip() + "\n\n" + completed_line + "\n"
    text = _bump_last_updated(text)
    write_project(path, text)
    print(f"{path.name}: marked Complete (date stamped {_TODAY})")
    print("Reminder: also update Planning/README.md status table count "
          "and TODO_Project_Extraction.md.")
    return 0


def cmd_link(args: argparse.Namespace) -> int:
    """Add reciprocal `## Related Documents` entries between two projects."""
    px = find_project(args.x)
    py = find_project(args.y)
    tx = read_project(px)
    ty = read_project(py)
    tx_title = project_title(tx)
    ty_title = project_title(ty)
    # link_in_x points from px's file to py; link_in_y points from py's file to px.
    link_in_x = relative_link_to(py, px)
    link_in_y = relative_link_to(px, py)

    def add_related(content: str, other_num: int, other_title: str,
                    other_link: str, note: str) -> tuple[str, bool]:
        entry_body = f"- **[Project {other_num} - {other_title}]({other_link})**"
        entry = f"{entry_body} — {note}" if note else entry_body

        lines = content.splitlines(keepends=True)
        for i, line in enumerate(lines):
            if re.match(r"^## Related Documents\s*$", line.rstrip("\r\n")):
                # Find the last `- ` bullet that belongs to this section.
                last_bullet = i
                j = i + 1
                while j < len(lines):
                    stripped = lines[j].rstrip("\r\n")
                    if stripped.startswith("## ") or stripped == "---":
                        break
                    if stripped.startswith("- "):
                        if other_link in stripped:
                            return content, False  # already linked
                        last_bullet = j
                    j += 1
                line_ending = "\r\n" if lines[last_bullet].endswith("\r\n") else "\n"
                # If no bullets exist yet, ensure a blank line separates header & entry.
                insert_at = last_bullet + 1
                if last_bullet == i:
                    lines.insert(insert_at, line_ending)
                    insert_at += 1
                lines.insert(insert_at, entry + line_ending)
                return "".join(lines), True

        # No section found — append a new one near the file end.
        line_ending = "\r\n" if content.endswith("\r\n") else "\n"
        suffix = (
            f"{line_ending}{line_ending}## Related Documents{line_ending}"
            f"{line_ending}{entry}{line_ending}"
        )
        return content.rstrip() + suffix, True

    tx_new, added_x = add_related(tx, args.y, ty_title, link_in_x, args.x_note or "")
    ty_new, added_y = add_related(ty, args.x, tx_title, link_in_y, args.y_note or "")

    tx_new = _bump_last_updated(tx_new) if added_x else tx_new
    ty_new = _bump_last_updated(ty_new) if added_y else ty_new

    write_project(px, tx_new)
    write_project(py, ty_new)

    print(f"{px.name}: {'added' if added_x else 'already had'} link to {py.name}")
    print(f"{py.name}: {'added' if added_y else 'already had'} link to {px.name}")
    return 0


def cmd_risk(args: argparse.Namespace) -> int:
    """Append a row to the Risks & Mitigations table."""
    path = find_project(args.number)
    text = read_project(path)

    lines = text.splitlines(keepends=True)
    in_section = False
    table_started = False
    last_table_idx = -1
    for i, raw in enumerate(lines):
        stripped = raw.rstrip("\r\n")
        if re.match(r"^## Risks & Mitigations\s*$", stripped):
            in_section = True
            continue
        if not in_section:
            continue
        if stripped.startswith("## "):
            break  # left the section
        if stripped.startswith("|"):
            table_started = True
            last_table_idx = i
        elif table_started and not stripped:
            break  # blank line after table ends the rows

    if last_table_idx < 0:
        print(f"error: no Risks & Mitigations table in {path.name}",
              file=sys.stderr)
        return 1

    line_ending = "\r\n" if lines[last_table_idx].endswith("\r\n") else "\n"
    new_row = (
        f"| **{args.title}** | {args.likelihood} | {args.impact} "
        f"| {args.mitigation} |{line_ending}"
    )
    lines.insert(last_table_idx + 1, new_row)
    text = "".join(lines)
    text = _bump_last_updated(text)
    write_project(path, text)
    print(f"{path.name}: added risk row {args.title!r}")
    return 0


# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------

def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.split("\n\n", 1)[0])
    sub = parser.add_subparsers(dest="cmd", required=True)

    p_status = sub.add_parser("status", help="Set Status field")
    p_status.add_argument("number", type=int)
    p_status.add_argument("status", choices=STATUS_CHOICES)
    p_status.set_defaults(func=cmd_status)

    p_priority = sub.add_parser("priority", help="Set Priority field")
    p_priority.add_argument("number", type=int)
    p_priority.add_argument("priority", choices=PRIORITY_CHOICES)
    p_priority.set_defaults(func=cmd_priority)

    p_complete = sub.add_parser("complete", help="Mark project Complete")
    p_complete.add_argument("number", type=int)
    p_complete.set_defaults(func=cmd_complete)

    p_link = sub.add_parser("link", help="Cross-reference two projects")
    p_link.add_argument("x", type=int, help="First project number")
    p_link.add_argument("y", type=int, help="Second project number")
    p_link.add_argument("--x-note", default="", help="Why X relates to Y")
    p_link.add_argument("--y-note", default="", help="Why Y relates to X")
    p_link.set_defaults(func=cmd_link)

    p_risk = sub.add_parser("risk", help="Add a Risks & Mitigations row")
    p_risk.add_argument("number", type=int)
    p_risk.add_argument("--title", required=True)
    p_risk.add_argument("--likelihood", required=True, choices=LIKELIHOOD_CHOICES)
    p_risk.add_argument("--impact", required=True, choices=IMPACT_CHOICES)
    p_risk.add_argument("--mitigation", required=True)
    p_risk.set_defaults(func=cmd_risk)

    args = parser.parse_args()
    return args.func(args)


if __name__ == "__main__":
    sys.exit(main())
