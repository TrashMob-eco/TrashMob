const fs = require("fs");
const path = require("path");

const CHECKLIST_PATH = path.resolve(
  __dirname,
  "../../PageReviewChecklist.md"
);

// Matches table rows like: | 1 | Home | `/` | [ ] | [ ] | notes here |
// Also handles: | M1 | Welcome / Sign In | `WelcomePage` | [ ] | [ ] | |
const ROW_RE =
  /^\|\s*([\dM]+\d*)\s*\|([^|]+)\|([^|]+)\|\s*\[([ x])\]\s*\|\s*\[([ x])\]\s*\|([^|]*)\|$/;

// Section header like: ## Website — Public Pages
const SECTION_RE = /^## (.+)$/;

function parseChecklist(filePath = CHECKLIST_PATH) {
  const content = fs.readFileSync(filePath, "utf-8");
  const lines = content.split("\n");
  const items = [];
  let currentSection = "";

  for (let i = 0; i < lines.length; i++) {
    const sectionMatch = lines[i].match(SECTION_RE);
    if (sectionMatch) {
      const heading = sectionMatch[1].trim();
      // Skip non-checklist sections (Table of Contents, Progress Summary)
      if (
        heading !== "Table of Contents" &&
        heading !== "Progress Summary" &&
        !heading.startsWith("TrashMob")
      ) {
        currentSection = heading;
      }
      continue;
    }

    const rowMatch = lines[i].match(ROW_RE);
    if (rowMatch) {
      const rawRoute = rowMatch[3].trim().replace(/`/g, "");
      items.push({
        id: rowMatch[1].trim(),
        page: rowMatch[2].trim(),
        route: rawRoute,
        looksGood: rowMatch[4] === "x",
        works: rowMatch[5] === "x",
        notes: rowMatch[6].trim(),
        section: currentSection,
        lineNumber: i,
      });
    }
  }

  return items;
}

function updateChecklistItem(id, updates, filePath = CHECKLIST_PATH) {
  const content = fs.readFileSync(filePath, "utf-8");
  const lines = content.split("\n");
  const items = parseChecklist(filePath);
  const item = items.find((it) => it.id === id);

  if (!item) {
    throw new Error(`Item with id "${id}" not found`);
  }

  const lg =
    updates.looksGood !== undefined ? updates.looksGood : item.looksGood;
  const w = updates.works !== undefined ? updates.works : item.works;
  const rawNotes = updates.notes !== undefined ? updates.notes : item.notes;
  // Strip newlines and pipe chars — they break markdown table rows
  const notes = rawNotes.replace(/[\r\n]+/g, " ").replace(/\|/g, "/").trim();

  // Reconstruct the table row preserving column alignment
  const lgStr = lg ? "[x]" : "[ ]";
  const wStr = w ? "[x]" : "[ ]";
  const notesStr = notes ? ` ${notes} ` : " ";

  lines[item.lineNumber] =
    `| ${item.id} | ${item.page} | ${item.route.includes("/") || item.route.includes("Page") ? "`" + item.route + "`" : item.route} | ${lgStr} | ${wStr} |${notesStr}|`;

  fs.writeFileSync(filePath, lines.join("\n"), "utf-8");

  return { ...item, looksGood: lg, works: w, notes: notes.trim() };
}

// Build full route patterns for items that use suffix-only routes
function resolveFullRoute(item) {
  const route = item.route;
  const section = item.section;

  // Community Management routes are suffixes of /partnerdashboard/:partnerId/...
  if (section === "Website — Community Management") {
    return `/partnerdashboard/:partnerId${route}`;
  }

  // Site Admin routes are suffixes of /siteadmin/...
  if (section === "Website — Site Admin") {
    return `/siteadmin${route}`;
  }

  return route;
}

// Convert a route pattern like /communities/:slug to a regex
function routeToRegex(routePattern) {
  // Escape special regex chars except : which we handle
  const escaped = routePattern
    .replace(/[.*+?^${}()|[\]\\]/g, "\\$&")
    .replace(/:[\w]+/g, "[^/]+");
  return new RegExp(`^${escaped}$`);
}

function matchRoute(urlPath, filePath = CHECKLIST_PATH) {
  const items = parseChecklist(filePath);
  let bestMatch = null;
  let bestScore = -1;

  for (const item of items) {
    // Skip mobile items and scroll sections (no real route)
    if (item.id.startsWith("M")) continue;
    if (item.route.startsWith("(")) continue;

    const fullRoute = resolveFullRoute(item);
    const regex = routeToRegex(fullRoute);

    if (regex.test(urlPath)) {
      // Score: exact matches get 100, parameterized matches get 50 + literal segment count
      const isExact = fullRoute === urlPath;
      const literalSegments = fullRoute
        .split("/")
        .filter((s) => s && !s.startsWith(":")).length;
      const score = isExact ? 100 : 50 + literalSegments;

      if (score > bestScore) {
        bestScore = score;
        bestMatch = item;
      }
    }
  }

  // For /mydashboard, also return the scroll sub-sections
  if (urlPath === "/mydashboard") {
    const subSections = items.filter(
      (it) =>
        it.section === "Website — User Dashboard & Account" &&
        it.route.startsWith("(")
    );
    return {
      primary: bestMatch,
      subSections,
    };
  }

  return { primary: bestMatch, subSections: [] };
}

function getStats(filePath = CHECKLIST_PATH) {
  const items = parseChecklist(filePath);
  const sections = {};

  for (const item of items) {
    if (!sections[item.section]) {
      sections[item.section] = { total: 0, looksGood: 0, works: 0, done: 0 };
    }
    sections[item.section].total++;
    if (item.looksGood) sections[item.section].looksGood++;
    if (item.works) sections[item.section].works++;
    if (item.looksGood && item.works) sections[item.section].done++;
  }

  const total = items.length;
  const done = items.filter((it) => it.looksGood && it.works).length;

  return { total, done, remaining: total - done, sections };
}

module.exports = {
  parseChecklist,
  updateChecklistItem,
  matchRoute,
  getStats,
  CHECKLIST_PATH,
};
