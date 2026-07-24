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

## Slide deck format: Marp

Decks are written in [Marp](https://marp.app/) markdown — plain `.md` files where `---` separates slides. Render to PowerPoint, PDF, or HTML:

```bash
# One-time install (choose one)
npm install -g @marp-team/marp-cli      # CLI
# or install "Marp for VS Code" extension for live preview

# Export
marp customers/Norwalk-CT/deck.md --pptx    # PowerPoint (.pptx)
marp customers/Norwalk-CT/deck.md --pdf     # PDF handout
marp customers/Norwalk-CT/deck.md --html    # Self-contained HTML
```

Why Marp instead of `.pptx` binaries:
- Diffable in git — reviewers can see exactly what changed
- Trivial to reuse content across customers (copy a slide, swap the city name)
- Free of PowerPoint's design drift between contributors

For a customer who insists on editing the deck themselves, export to `.pptx` and hand that off.

## One-pagers

One-pagers are plain markdown, ready to render to PDF via any markdown-to-PDF tool (VS Code's built-in export, Pandoc, or `marp --pdf` if you add a Marp header). They should fit on a single US Letter page when printed.

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
