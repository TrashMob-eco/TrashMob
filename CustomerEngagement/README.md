# Customer Engagement

Materials tailored to specific customers, prospects, and partner conversations — one-pagers, slide decks, pre-call briefs, and follow-up templates. Everything here is version-controlled so we can iterate as we learn what resonates.

## Folder layout

```
CustomerEngagement/
├── README.md                 # This file
├── templates/                # Reusable base templates — start here for a new customer
│   ├── one-pager-template.md
│   └── deck-template.md
└── customers/                # One folder per customer / prospect
    └── {City-State}/         # e.g. Norwalk-CT/
        ├── pre-call-brief.md # Internal: research + talking points before the call
        ├── one-pager.md      # External: leave-behind for the customer
        └── deck.md           # External: slide deck (Marp format — see below)
```

## Source of truth is markdown

All docs here — one-pagers, decks, briefs — are plain markdown. Generated binaries (`.docx`, `.pptx`, `.pdf`) are `.gitignored` and regenerated on demand. This keeps diffs readable and avoids format drift.

## Exporting to `.docx` (one-pagers, briefs) — Pandoc

```powershell
# One-time install
winget install --id=JohnMacFarlane.Pandoc --accept-source-agreements --accept-package-agreements

# Export
$pandoc = "$env:LOCALAPPDATA\Pandoc\pandoc.exe"       # or just "pandoc" once your PATH picks it up
& $pandoc customers\Norwalk-CT\one-pager.md      -o customers\Norwalk-CT\one-pager.docx      --from=gfm --to=docx
& $pandoc customers\Norwalk-CT\pre-call-brief.md -o customers\Norwalk-CT\pre-call-brief.docx --from=gfm --to=docx
```

## Exporting the deck to `.pptx`

**Option A — Pandoc** (no extra deps, plain slides):

```powershell
& $pandoc customers\Norwalk-CT\deck.md -o customers\Norwalk-CT\deck.pptx --from=gfm --to=pptx --slide-level=2
```

**Option B — Marp** (nicer styling, matches the Marp preview in VS Code):

```powershell
# One-time install (downloads chromium on first run — ~150 MB)
npm install -g @marp-team/marp-cli
# or install "Marp for VS Code" extension for live preview

marp customers\Norwalk-CT\deck.md --pptx    # PowerPoint
marp customers\Norwalk-CT\deck.md --pdf     # PDF handout
marp customers\Norwalk-CT\deck.md --html    # Self-contained HTML
```

Why markdown-first instead of maintaining `.pptx` / `.docx` in git:
- Diffable — reviewers see exactly what changed
- Trivial to reuse content across customers (copy a slide, swap the city name)
- No PowerPoint / Word design drift between contributors

For a customer who insists on editing the deck themselves, export to `.pptx` and hand that off — that's a one-way handoff, and further edits happen in their file.

## Creating materials for a new customer

1. Copy `templates/` files into `customers/{City-State}/`.
2. Write a `pre-call-brief.md` first — do the research, capture their existing programs, name the person you're meeting, note their priorities. This is internal-only.
3. Tailor the one-pager and deck to their specific gap. Generic pitches lose to specific ones.
4. After the call, add a `call-notes-{YYYY-MM-DD}.md` file with what was said, what they asked for, and next steps. This becomes the memory for future conversations.

## What lives here vs. elsewhere

- **Product features and roadmap** → `/Planning/README.md` (source of truth). Link to it from customer materials rather than restating.
- **Onboarding and self-service docs** → `/Support/*.md`. Point customers here after they say yes.
- **Marketing site copy** → `TrashMob/client-app/` (React) or Strapi CMS. Not here.
- **Legal / waivers / contracts** → not in this repo. Coordinate with info@trashmob.eco.

## Do not commit

- Signed contracts, private customer data, or anything the customer hasn't approved for open source
- Meeting recordings or transcripts with PII
- Anything under NDA

If unsure, ask before pushing.
