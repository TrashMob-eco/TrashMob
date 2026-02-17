# Page Review Tracker

A Chrome extension + local server that helps you track your page-by-page review of dev.trashmob.eco. As you browse the site, a floating panel auto-detects which page you're on and lets you mark it as "Looks Good" and/or "Works", plus add notes. All changes are saved directly to `Testing/PageReviewChecklist.md`.

## Quick Start

### 1. Start the local server

```bash
cd Testing/review-tool/server
npm install
npm start
```

You should see:
```
Page Review Tracker server running on http://localhost:3456
Checklist: 0/176 pages fully reviewed (176 remaining)
```

### 2. Load the Chrome extension

1. Open Chrome and navigate to `chrome://extensions/`
2. Enable **Developer mode** (toggle in the top-right corner)
3. Click **Load unpacked**
4. Select the folder: `Testing/review-tool/extension/`
5. The "TrashMob Page Review Tracker" extension should appear in your extensions list

### 3. Start reviewing

1. Navigate to https://dev.trashmob.eco
2. A green floating panel appears in the bottom-right corner
3. The panel auto-detects the current page and shows the matching checklist item
4. Click **Looks Good** (green) or **Works** (blue) to check them off
5. Type notes in the text field — they auto-save after 1 second
6. Navigate to another page — the panel updates automatically

## How It Works

```
Chrome Extension                    Local Server (port 3456)
┌─────────────────┐                ┌──────────────────────┐
│ Detects URL     │───GET /match──▶│ Finds matching item  │
│ Shows panel     │◀──────────────│ in PageReviewChecklist│
│ User clicks     │──PATCH /items─▶│ Updates markdown file│
│ checkbox/notes  │◀──────────────│ Returns updated item │
└─────────────────┘                └──────────────────────┘
```

- The extension runs as a **content script** on `dev.trashmob.eco`
- It intercepts React Router navigation (pushState/popstate) so the panel updates without page reloads
- The panel uses **Shadow DOM** to avoid any style conflicts with the site
- All state is stored in `Testing/PageReviewChecklist.md` — you can also edit it by hand

## Panel Features

- **Auto-detection**: Matches the current URL path to checklist routes (handles parameterized routes like `/communities/:slug`)
- **Looks Good / Works buttons**: Toggle with one click, saved immediately
- **Notes field**: Free-text, auto-saves on blur or after 1 second of inactivity
- **Section progress**: Shows "X/Y done in section" in the footer
- **Draggable**: Drag the green header bar to reposition the panel (position persists)
- **Collapsible**: Click the minimize button to shrink to a small green circle; click it to expand
- **Server status**: Green dot = connected, red dot = server offline
- **Dashboard sub-sections**: When on `/mydashboard`, shows checkboxes for each scroll section

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/items` | All checklist items |
| GET | `/api/match?path=/aboutus` | Find item matching a URL path |
| PATCH | `/api/items/:id` | Update LG, Works, or notes for an item |
| GET | `/api/stats` | Progress summary by section |
| GET | `/api/health` | Server health check |

## File Structure

```
Testing/review-tool/
├── server/
│   ├── package.json           # Dependencies (express, cors)
│   ├── server.js              # Express server on port 3456
│   └── markdown-parser.js     # Reads/writes PageReviewChecklist.md
├── extension/
│   ├── manifest.json          # Chrome Manifest V3
│   ├── content.js             # Injected panel + logic
│   └── icons/                 # Extension icons
├── generate-icons.js          # Script that generated the icons
└── README.md                  # This file
```

## Troubleshooting

**Panel doesn't appear:**
- Make sure the local server is running (`npm start` in the server folder)
- Check that the extension is loaded and enabled in `chrome://extensions/`
- The extension only activates on `https://dev.trashmob.eco/*`

**"Server offline" message:**
- Run `cd Testing/review-tool/server && npm start`
- Check that port 3456 is not in use by another process

**Changes not saving:**
- Check the server terminal for error messages
- Verify `Testing/PageReviewChecklist.md` is not open with exclusive file lock

**Route not matching:**
- Some routes use `:param` placeholders (e.g., `/communities/:slug`)
- Community Management and Site Admin routes are matched with their full paths
- Mobile items (M1–M46) don't auto-match since they're in the mobile app, not the website
