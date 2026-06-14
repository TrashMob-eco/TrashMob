#!/usr/bin/env python3
"""
Scaffold a new project plan in Planning/Projects/.

Replaces the deterministic prefix of the LLM "add a new project" workflow
(find next number, build the filename, copy the template, fill in trivial
header fields, set today's date). The creative work — Business Rationale,
Scope, Risks, Open Questions — is left to the human or LLM as a follow-up.

Usage:
    python scripts/new_project.py "Title of the project" \\
        [--priority Low|Medium|High|Critical] \\
        [--size  Very-Small|Small|Medium|Large|Very-Large] \\
        [--risk  Low|Medium|High|Very-High] \\
        [--status Not-Started|Planning] \\
        [--owner "Team or person"] \\
        [--dependencies "Project 5, Project 11"] \\
        [--archive] \\
        [--print-followups]

`--archive` writes to Planning/Projects/Archive/ instead. Useful for
projects that are already complete on day one (e.g. retroactive docs).

`--print-followups` lists the non-mechanical updates a human needs to
make after this script runs: README.md status table, Executive_Summary,
TODO list, Risks_and_Mitigations entry if risk is High+, etc. Defaults
to on.
"""

from __future__ import annotations

import argparse
import datetime
import re
import sys
from pathlib import Path

PLANNING = Path(__file__).resolve().parents[1] / "Planning"
PROJECTS = PLANNING / "Projects"
ARCHIVE = PROJECTS / "Archive"
TEMPLATE = PROJECTS / "_Project_Template.md"


def next_project_number() -> int:
    nums: list[int] = []
    for src in (PROJECTS, ARCHIVE):
        for path in src.glob("Project_*.md"):
            m = re.match(r"Project_(\d+)_", path.name)
            if m:
                nums.append(int(m.group(1)))
    return (max(nums) + 1) if nums else 1


def title_to_filename_segment(title: str) -> str:
    """Convert 'My Cool Project!' → 'My_Cool_Project'."""
    cleaned = re.sub(r"[^\w\s-]", "", title)        # drop punctuation
    cleaned = re.sub(r"[\s-]+", "_", cleaned).strip("_")
    return cleaned or "Untitled"


def _read_template() -> str:
    """Read the template tolerant of the historical cp1252 file encoding.

    Project templates have been around since pre-utf8-everywhere; we accept
    the legacy encoding on input and normalise to utf-8 on output.
    """
    raw = TEMPLATE.read_bytes()
    for encoding in ("utf-8", "utf-8-sig", "cp1252"):
        try:
            return raw.decode(encoding)
        except UnicodeDecodeError:
            continue
    raise UnicodeDecodeError(
        "utf-8", raw, 0, 1,
        f"could not decode {TEMPLATE} as utf-8 / utf-8-sig / cp1252",
    )


def render_project(
    number: int,
    title: str,
    priority: str,
    size: str,
    risk: str,
    status: str,
    owner: str,
    dependencies: str,
) -> str:
    today = datetime.date.today().isoformat()
    template = _read_template()

    # The template's first line is "# Project Template — [PROJECT TITLE]".
    # Swap to "# Project N — Title".
    template = re.sub(
        r"^# Project Template.*$",
        f"# Project {number} — {title}",
        template,
        count=1,
        flags=re.MULTILINE,
    )

    # Header table values — only the bracketed placeholders we know how to fill.
    template = re.sub(
        r"^\| \*\*Status\*\* \| \[Not Started.*?\] \|",
        f"| **Status** | {status} |",
        template,
        count=1,
        flags=re.MULTILINE,
    )
    template = re.sub(
        r"^\| \*\*Priority\*\* \| \[Low.*?\] \|",
        f"| **Priority** | {priority} |",
        template,
        count=1,
        flags=re.MULTILINE,
    )
    template = re.sub(
        r"^\| \*\*Risk\*\* \| \[Low.*?\] \|",
        f"| **Risk** | {risk} |",
        template,
        count=1,
        flags=re.MULTILINE,
    )
    template = re.sub(
        r"^\| \*\*Size\*\* \| \[Very Small.*?\] \|",
        f"| **Size** | {size} |",
        template,
        count=1,
        flags=re.MULTILINE,
    )
    template = re.sub(
        r"^\| \*\*Dependencies\*\* \| \[List of dependent projects\] \|",
        f"| **Dependencies** | {dependencies or '_None_'} |",
        template,
        count=1,
        flags=re.MULTILINE,
    )

    # Footer trailer.
    template = re.sub(r"\*\*Last Updated:\*\* \[Date\]", f"**Last Updated:** {today}", template)
    template = re.sub(r"\*\*Owner:\*\* \[Team/Person responsible\]", f"**Owner:** {owner}", template)
    template = re.sub(r"\*\*Status:\*\* \[Current status\]", f"**Status:** {status}", template)
    template = re.sub(
        r"\*\*Next Review:\*\* \[When to review again\]",
        f"**Next Review:** {(datetime.date.today() + datetime.timedelta(days=30)).isoformat()}",
        template,
    )
    return template


FOLLOWUPS = """
After scaffolding, the human (or follow-up LLM pass) still needs to:

  1. Fill in Business Rationale, Objectives, Scope (phases), Out-of-Scope,
     Success Metrics, Risks & Mitigations, Implementation Plan, Open Questions.

  2. Update Planning/README.md
     - Add the project to the appropriate quarter/theme section
     - Bump the "Not Started" count in the status table

  3. Update Planning/.claude_commands_index.md if a new common operation
     was introduced.

  4. Update Planning/Executive_Summary.md if the project is high-priority
     or large.

  5. If Risk is High or Very High, add a PR-{N} entry to
     Planning/Risks_and_Mitigations.md.

  6. Link this project from any pre-existing related projects'
     "Related Documents" sections.
"""


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.split("\n\n", 1)[0])
    parser.add_argument("title", help="Project title, e.g. 'Email Digest System'")
    parser.add_argument("--priority", default="Medium",
                        choices=["Low", "Medium", "High", "Critical"])
    parser.add_argument("--size", default="Medium",
                        choices=["Very Small", "Small", "Medium", "Large", "Very Large",
                                 "Very-Small", "Very-Large"])  # dash form for CLI ergonomics
    parser.add_argument("--risk", default="Low",
                        choices=["Low", "Medium", "High", "Very High", "Very-High"])
    parser.add_argument("--status", default="Not Started",
                        choices=["Not Started", "Not-Started", "Planning"])
    parser.add_argument("--owner", default="TBD")
    parser.add_argument("--dependencies", default="",
                        help="Free-text dependencies list, e.g. 'Project 11, Project 23'")
    parser.add_argument("--archive", action="store_true",
                        help="Write to Planning/Projects/Archive/ instead of Projects/")
    parser.add_argument("--no-followups", action="store_true",
                        help="Don't print the follow-up checklist after scaffolding")
    parser.add_argument("--dry-run", action="store_true",
                        help="Print the proposed filename + content; don't write")
    args = parser.parse_args()

    # Normalise the dashed CLI variants back to space form.
    size = args.size.replace("-", " ")
    risk = args.risk.replace("-", " ")
    status = args.status.replace("-", " ")

    if not TEMPLATE.exists():
        print(f"error: template not found at {TEMPLATE}", file=sys.stderr)
        return 2

    number = next_project_number()
    filename_segment = title_to_filename_segment(args.title)
    filename = f"Project_{number}_{filename_segment}.md"
    target_dir = ARCHIVE if args.archive else PROJECTS
    target_dir.mkdir(parents=True, exist_ok=True)
    target = target_dir / filename

    if target.exists():
        print(f"error: {target} already exists", file=sys.stderr)
        return 1

    rendered = render_project(
        number=number,
        title=args.title,
        priority=args.priority,
        size=size,
        risk=risk,
        status=status,
        owner=args.owner,
        dependencies=args.dependencies,
    )

    if args.dry_run:
        print(f"# Would write: {target}")
        print(rendered)
        return 0

    target.write_text(rendered, encoding="utf-8")
    print(f"Created {target}")
    print(f"  Project number: {number}")
    print(f"  Title:          {args.title}")
    print(f"  Priority/Size/Risk: {args.priority} / {size} / {risk}")
    print(f"  Status:         {status}")
    if args.dependencies:
        print(f"  Dependencies:   {args.dependencies}")

    if not args.no_followups:
        print(FOLLOWUPS)
    return 0


if __name__ == "__main__":
    sys.exit(main())
