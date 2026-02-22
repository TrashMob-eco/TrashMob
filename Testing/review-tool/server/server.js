const express = require("express");
const cors = require("cors");
const {
  parseChecklist,
  updateChecklistItem,
  matchRoute,
  getStats,
} = require("./markdown-parser");

const app = express();
const PORT = 3456;

app.use(
  cors({
    origin: [/chrome-extension:\/\/.*/, /https?:\/\/dev\.trashmob\.eco/],
    methods: ["GET", "PATCH", "OPTIONS"],
  })
);
app.use(express.json());

// GET /api/items — return all checklist items
app.get("/api/items", (req, res) => {
  try {
    const items = parseChecklist();
    res.json({ items, count: items.length });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// GET /api/match?path=/communities/seattle — find matching item for a URL path
app.get("/api/match", (req, res) => {
  const urlPath = req.query.path;
  if (!urlPath) {
    return res.status(400).json({ error: "Missing ?path= query parameter" });
  }

  try {
    const result = matchRoute(urlPath);
    if (!result.primary) {
      return res.json({ matched: false, path: urlPath });
    }

    // Get section stats for the matched item
    const stats = getStats();
    const sectionStats = stats.sections[result.primary.section] || {};

    res.json({
      matched: true,
      path: urlPath,
      item: result.primary,
      subSections: result.subSections,
      sectionStats,
    });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// PATCH /api/items/:id — update LG, W, notes for an item
app.patch("/api/items/:id", (req, res) => {
  const { id } = req.params;
  const { looksGood, works, notes } = req.body;

  try {
    const updated = updateChecklistItem(id, { looksGood, works, notes });
    res.json({ success: true, item: updated });
  } catch (err) {
    res.status(err.message.includes("not found") ? 404 : 500).json({
      error: err.message,
    });
  }
});

// GET /api/stats — return summary counts
app.get("/api/stats", (req, res) => {
  try {
    const stats = getStats();
    res.json(stats);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// Health check
app.get("/api/health", (req, res) => {
  res.json({ status: "ok" });
});

app.listen(PORT, () => {
  console.log(`Page Review Tracker server running on http://localhost:${PORT}`);
  console.log(`Endpoints:`);
  console.log(`  GET  /api/items        — all checklist items`);
  console.log(`  GET  /api/match?path=  — find item matching a URL path`);
  console.log(`  PATCH /api/items/:id   — update an item`);
  console.log(`  GET  /api/stats        — progress summary`);
  console.log(`  GET  /api/health       — health check`);

  // Show initial stats
  try {
    const stats = getStats();
    console.log(
      `\nChecklist: ${stats.done}/${stats.total} pages fully reviewed (${stats.remaining} remaining)`
    );
  } catch (err) {
    console.error("Error reading checklist:", err.message);
  }
});
