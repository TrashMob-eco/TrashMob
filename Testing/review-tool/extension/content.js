// TrashMob Page Review Tracker — Chrome Extension Content Script
// Injects a floating overlay on dev.trashmob.eco that communicates with
// a local Node.js server to track page review progress.

(function () {
  "use strict";

  const API_BASE = "http://localhost:3456/api";
  const COLLAPSED_KEY = "tm-review-collapsed";
  const POSITION_KEY = "tm-review-position";

  let currentItem = null;
  let subSections = [];
  let sectionStats = null;
  let serverOnline = false;
  let saveTimeout = null;

  // ── Shadow DOM host ──────────────────────────────────────────────

  const host = document.createElement("div");
  host.id = "tm-review-tracker-host";
  host.style.cssText =
    "all:initial; position:fixed; z-index:2147483647; pointer-events:none;";
  document.body.appendChild(host);
  const shadow = host.attachShadow({ mode: "closed" });

  // ── Styles ───────────────────────────────────────────────────────

  const style = document.createElement("style");
  style.textContent = `
    * { box-sizing: border-box; margin: 0; padding: 0; }

    .panel {
      position: fixed;
      bottom: 16px;
      right: 16px;
      width: 340px;
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
      font-size: 13px;
      color: #1a1a1a;
      background: #ffffff;
      border: 1px solid #d1d5db;
      border-radius: 12px;
      box-shadow: 0 8px 30px rgba(0,0,0,0.12), 0 2px 8px rgba(0,0,0,0.06);
      pointer-events: auto;
      user-select: none;
      overflow: hidden;
      transition: width 0.2s, height 0.2s;
    }

    .panel.collapsed {
      width: auto;
      border-radius: 50%;
    }

    /* ── Header (drag handle) ── */
    .header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 8px 12px;
      background: #0e7c3a;
      color: #fff;
      cursor: grab;
      gap: 8px;
    }
    .header:active { cursor: grabbing; }
    .header-title {
      font-weight: 600;
      font-size: 12px;
      letter-spacing: 0.3px;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      flex: 1;
    }
    .header-controls { display: flex; gap: 4px; align-items: center; }
    .header-btn {
      background: none;
      border: none;
      color: #fff;
      cursor: pointer;
      font-size: 16px;
      line-height: 1;
      padding: 2px 4px;
      border-radius: 4px;
      opacity: 0.85;
    }
    .header-btn:hover { opacity: 1; background: rgba(255,255,255,0.15); }

    /* ── Status dot ── */
    .status-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      flex-shrink: 0;
    }
    .status-dot.online { background: #4ade80; }
    .status-dot.offline { background: #f87171; }

    /* ── Body ── */
    .body { padding: 12px; }
    .body.hidden { display: none; }

    /* ── Section & Page info ── */
    .section-label {
      font-size: 10px;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      color: #6b7280;
      margin-bottom: 2px;
    }
    .page-name {
      font-size: 16px;
      font-weight: 700;
      color: #111;
      margin-bottom: 2px;
    }
    .route-label {
      font-size: 11px;
      color: #9ca3af;
      font-family: monospace;
      margin-bottom: 10px;
    }

    /* ── Toggle buttons ── */
    .toggles {
      display: flex;
      gap: 8px;
      margin-bottom: 10px;
    }
    .toggle-btn {
      flex: 1;
      padding: 8px 0;
      border: 2px solid #d1d5db;
      border-radius: 8px;
      background: #f9fafb;
      cursor: pointer;
      font-size: 13px;
      font-weight: 600;
      font-family: inherit;
      color: #6b7280;
      transition: all 0.15s;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 6px;
    }
    .toggle-btn:hover { border-color: #9ca3af; }
    .toggle-btn.checked-lg {
      background: #dcfce7;
      border-color: #22c55e;
      color: #15803d;
    }
    .toggle-btn.checked-w {
      background: #dbeafe;
      border-color: #3b82f6;
      color: #1d4ed8;
    }
    .toggle-icon { font-size: 16px; }

    /* ── Notes ── */
    .notes-field {
      width: 100%;
      height: 34px;
      padding: 6px 10px;
      border: 1px solid #d1d5db;
      border-radius: 8px;
      font-family: inherit;
      font-size: 12px;
      color: #1a1a1a;
      outline: none;
      transition: border-color 0.15s;
    }
    .notes-field:focus { border-color: #3b82f6; }
    .notes-field::placeholder { color: #9ca3af; }

    /* ── Footer (progress + nav) ── */
    .footer {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 8px 12px;
      border-top: 1px solid #e5e7eb;
      font-size: 11px;
      color: #6b7280;
    }
    .progress-text { font-weight: 500; }
    .save-indicator {
      font-size: 11px;
      color: #22c55e;
      font-weight: 600;
      opacity: 0;
      transition: opacity 0.3s;
    }
    .save-indicator.visible { opacity: 1; }

    /* ── No match state ── */
    .no-match {
      text-align: center;
      padding: 16px 12px;
      color: #6b7280;
    }
    .no-match-path {
      font-family: monospace;
      font-size: 11px;
      color: #9ca3af;
      margin-top: 4px;
      word-break: break-all;
    }

    /* ── Sub-sections (for /mydashboard) ── */
    .sub-sections { margin-top: 10px; border-top: 1px solid #e5e7eb; padding-top: 8px; }
    .sub-section-title {
      font-size: 10px;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      color: #6b7280;
      margin-bottom: 6px;
    }
    .sub-item {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 4px 0;
      font-size: 12px;
      color: #374151;
    }
    .sub-check {
      width: 16px; height: 16px;
      border: 1.5px solid #d1d5db;
      border-radius: 4px;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 10px;
      flex-shrink: 0;
      background: #fff;
      transition: all 0.15s;
    }
    .sub-check.checked {
      background: #22c55e;
      border-color: #22c55e;
      color: #fff;
    }
    .sub-check:hover { border-color: #9ca3af; }
    .sub-label { flex: 1; }

    /* ── Collapsed badge ── */
    .collapsed-badge {
      width: 44px;
      height: 44px;
      border-radius: 50%;
      background: #0e7c3a;
      color: #fff;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      font-size: 18px;
      font-weight: 700;
      pointer-events: auto;
      box-shadow: 0 4px 12px rgba(0,0,0,0.15);
      position: fixed;
      bottom: 16px;
      right: 16px;
    }
    .collapsed-badge.has-match { background: #0e7c3a; }
    .collapsed-badge.no-match { background: #6b7280; }
    .collapsed-badge:hover { transform: scale(1.08); }
  `;
  shadow.appendChild(style);

  // ── Panel HTML ───────────────────────────────────────────────────

  const panel = document.createElement("div");
  panel.className = "panel";
  panel.innerHTML = `
    <div class="header" id="drag-handle">
      <div class="status-dot offline" id="status-dot" title="Server offline"></div>
      <div class="header-title">Page Review Tracker</div>
      <div class="header-controls">
        <span class="save-indicator" id="save-indicator">Saved</span>
        <button class="header-btn" id="btn-collapse" title="Minimize">&#x2212;</button>
      </div>
    </div>
    <div class="body" id="panel-body">
      <div id="content-area"></div>
    </div>
    <div class="footer" id="panel-footer">
      <span class="progress-text" id="progress-text"></span>
    </div>
  `;
  shadow.appendChild(panel);

  // Collapsed badge (hidden by default)
  const badge = document.createElement("div");
  badge.className = "collapsed-badge";
  badge.textContent = "R";
  badge.title = "Expand Page Review Tracker";
  badge.style.display = "none";
  badge.addEventListener("click", () => toggleCollapse(false));
  shadow.appendChild(badge);

  // ── DOM references ───────────────────────────────────────────────

  const statusDot = shadow.getElementById("status-dot");
  const contentArea = shadow.getElementById("content-area");
  const progressText = shadow.getElementById("progress-text");
  const saveIndicator = shadow.getElementById("save-indicator");
  const panelBody = shadow.getElementById("panel-body");
  const panelFooter = shadow.getElementById("panel-footer");
  const btnCollapse = shadow.getElementById("btn-collapse");
  const dragHandle = shadow.getElementById("drag-handle");

  // ── Collapse / Expand ────────────────────────────────────────────

  let isCollapsed = localStorage.getItem(COLLAPSED_KEY) === "true";

  function toggleCollapse(collapsed) {
    isCollapsed = collapsed;
    localStorage.setItem(COLLAPSED_KEY, collapsed);
    if (collapsed) {
      panel.style.display = "none";
      badge.style.display = "flex";
      badge.className =
        "collapsed-badge " + (currentItem ? "has-match" : "no-match");
      if (currentItem) {
        badge.textContent =
          currentItem.looksGood && currentItem.works ? "\u2713" : "R";
      }
    } else {
      panel.style.display = "";
      badge.style.display = "none";
    }
  }

  btnCollapse.addEventListener("click", () => toggleCollapse(true));

  // Apply initial collapsed state
  if (isCollapsed) toggleCollapse(true);

  // ── Dragging ─────────────────────────────────────────────────────

  let isDragging = false;
  let dragStartX, dragStartY, panelStartX, panelStartY;

  dragHandle.addEventListener("mousedown", (e) => {
    isDragging = true;
    dragStartX = e.clientX;
    dragStartY = e.clientY;
    const rect = panel.getBoundingClientRect();
    panelStartX = rect.left;
    panelStartY = rect.top;
    e.preventDefault();
  });

  document.addEventListener("mousemove", (e) => {
    if (!isDragging) return;
    const dx = e.clientX - dragStartX;
    const dy = e.clientY - dragStartY;
    panel.style.left = panelStartX + dx + "px";
    panel.style.top = panelStartY + dy + "px";
    panel.style.right = "auto";
    panel.style.bottom = "auto";
  });

  document.addEventListener("mouseup", () => {
    if (isDragging) {
      isDragging = false;
      // Save position
      const rect = panel.getBoundingClientRect();
      localStorage.setItem(
        POSITION_KEY,
        JSON.stringify({ left: rect.left, top: rect.top })
      );
    }
  });

  // Restore saved position
  try {
    const saved = JSON.parse(localStorage.getItem(POSITION_KEY));
    if (saved) {
      panel.style.left = saved.left + "px";
      panel.style.top = saved.top + "px";
      panel.style.right = "auto";
      panel.style.bottom = "auto";
    }
  } catch (e) {
    // ignore
  }

  // ── API helpers ──────────────────────────────────────────────────

  async function apiGet(path) {
    const res = await fetch(`${API_BASE}${path}`);
    if (!res.ok) throw new Error(`API ${res.status}`);
    return res.json();
  }

  async function apiPatch(id, body) {
    const res = await fetch(`${API_BASE}/items/${id}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body),
    });
    if (!res.ok) throw new Error(`API ${res.status}`);
    return res.json();
  }

  async function checkServer() {
    try {
      await apiGet("/health");
      serverOnline = true;
      statusDot.className = "status-dot online";
      statusDot.title = "Server connected";
    } catch {
      serverOnline = false;
      statusDot.className = "status-dot offline";
      statusDot.title = "Server offline — start the local server";
    }
    return serverOnline;
  }

  function flashSaved() {
    saveIndicator.classList.add("visible");
    setTimeout(() => saveIndicator.classList.remove("visible"), 1500);
  }

  // ── Render ───────────────────────────────────────────────────────

  function renderMatch(item, subs, stats) {
    currentItem = item;
    subSections = subs;
    sectionStats = stats;

    contentArea.innerHTML = `
      <div class="section-label">${item.section}</div>
      <div class="page-name">#${item.id} — ${item.page}</div>
      <div class="route-label">${item.route}</div>
      <div class="toggles">
        <button class="toggle-btn ${item.looksGood ? "checked-lg" : ""}" id="btn-lg">
          <span class="toggle-icon">${item.looksGood ? "\u2713" : "\u25CB"}</span>
          Looks Good
        </button>
        <button class="toggle-btn ${item.works ? "checked-w" : ""}" id="btn-w">
          <span class="toggle-icon">${item.works ? "\u2713" : "\u25CB"}</span>
          Works
        </button>
      </div>
      <input type="text" class="notes-field" id="notes-field"
        placeholder="Add notes..." value="${item.notes.replace(/"/g, "&quot;")}" />
    `;

    // Sub-sections for dashboard
    if (subs && subs.length > 0) {
      const subsDiv = document.createElement("div");
      subsDiv.className = "sub-sections";
      subsDiv.innerHTML = `<div class="sub-section-title">Dashboard Sections</div>`;
      for (const sub of subs) {
        const row = document.createElement("div");
        row.className = "sub-item";
        row.innerHTML = `
          <div class="sub-check ${sub.looksGood ? "checked" : ""}"
               data-id="${sub.id}" data-field="looksGood"
               title="Looks Good">${sub.looksGood ? "\u2713" : ""}</div>
          <div class="sub-check ${sub.works ? "checked" : ""}"
               data-id="${sub.id}" data-field="works"
               title="Works">${sub.works ? "\u2713" : ""}</div>
          <div class="sub-label">${sub.page}</div>
        `;
        subsDiv.appendChild(row);
      }
      contentArea.appendChild(subsDiv);

      // Sub-section check handlers
      subsDiv.querySelectorAll(".sub-check").forEach((el) => {
        el.addEventListener("click", async () => {
          const subId = el.dataset.id;
          const field = el.dataset.field;
          const sub = subs.find((s) => s.id === subId);
          if (!sub) return;
          const newVal = !sub[field];
          sub[field] = newVal;
          el.classList.toggle("checked", newVal);
          el.textContent = newVal ? "\u2713" : "";
          try {
            await apiPatch(subId, { [field]: newVal });
            flashSaved();
          } catch {
            // revert on failure
            sub[field] = !newVal;
            el.classList.toggle("checked", !newVal);
            el.textContent = !newVal ? "\u2713" : "";
          }
        });
      });
    }

    // Progress
    if (stats) {
      progressText.textContent = `${stats.done}/${stats.total} done in section`;
    }

    // Button handlers
    shadow.getElementById("btn-lg").addEventListener("click", async () => {
      item.looksGood = !item.looksGood;
      renderMatch(item, subs, stats);
      try {
        const result = await apiPatch(item.id, { looksGood: item.looksGood });
        sectionStats = (await apiGet(`/match?path=${encodeURIComponent(window.location.pathname)}`)).sectionStats;
        renderMatch(item, subs, sectionStats);
        flashSaved();
      } catch {
        item.looksGood = !item.looksGood;
        renderMatch(item, subs, stats);
      }
    });

    shadow.getElementById("btn-w").addEventListener("click", async () => {
      item.works = !item.works;
      renderMatch(item, subs, stats);
      try {
        const result = await apiPatch(item.id, { works: item.works });
        sectionStats = (await apiGet(`/match?path=${encodeURIComponent(window.location.pathname)}`)).sectionStats;
        renderMatch(item, subs, sectionStats);
        flashSaved();
      } catch {
        item.works = !item.works;
        renderMatch(item, subs, stats);
      }
    });

    // Notes auto-save
    // Strip newlines and pipe chars — they break markdown table rows
    function sanitizeNotes(val) {
      return val.replace(/[\r\n]+/g, " ").replace(/\|/g, "/").trim();
    }

    const notesField = shadow.getElementById("notes-field");
    notesField.addEventListener("input", () => {
      clearTimeout(saveTimeout);
      saveTimeout = setTimeout(async () => {
        const clean = sanitizeNotes(notesField.value);
        try {
          await apiPatch(item.id, { notes: clean });
          item.notes = clean;
          flashSaved();
        } catch {
          // silent fail, user can retry
        }
      }, 1000);
    });

    notesField.addEventListener("blur", async () => {
      clearTimeout(saveTimeout);
      const clean = sanitizeNotes(notesField.value);
      if (clean !== item.notes) {
        try {
          await apiPatch(item.id, { notes: clean });
          item.notes = clean;
          flashSaved();
        } catch {
          // silent fail
        }
      }
    });
  }

  function renderNoMatch(urlPath) {
    currentItem = null;
    subSections = [];
    contentArea.innerHTML = `
      <div class="no-match">
        No checklist item matches this route
        <div class="no-match-path">${urlPath}</div>
      </div>
    `;
    progressText.textContent = "";
  }

  function renderOffline() {
    contentArea.innerHTML = `
      <div class="no-match">
        Server offline<br/>
        <span style="font-size:11px; margin-top:8px; display:block;">
          Run: <code style="background:#f3f4f6; padding:2px 6px; border-radius:4px;">
          cd Testing/review-tool/server && npm start</code>
        </span>
      </div>
    `;
    progressText.textContent = "";
  }

  // ── Fetch & render current page ──────────────────────────────────

  async function updateForCurrentPage() {
    const online = await checkServer();
    if (!online) {
      renderOffline();
      return;
    }

    const urlPath = window.location.pathname;
    try {
      const data = await apiGet(`/match?path=${encodeURIComponent(urlPath)}`);
      if (data.matched) {
        renderMatch(data.item, data.subSections || [], data.sectionStats);
      } else {
        renderNoMatch(urlPath);
      }
    } catch {
      renderOffline();
    }
  }

  // ── SPA navigation detection ─────────────────────────────────────
  // Content scripts run in an isolated world, so monkey-patching
  // history.pushState doesn't intercept React Router calls.
  // Instead, poll the URL and update when it changes.

  let lastPath = window.location.pathname;

  setInterval(() => {
    const currentPath = window.location.pathname;
    if (currentPath !== lastPath) {
      lastPath = currentPath;
      updateForCurrentPage();
    }
    // Also retry server connection if offline
    if (!serverOnline) updateForCurrentPage();
  }, 500);

  window.addEventListener("popstate", () => {
    setTimeout(updateForCurrentPage, 50);
  });

  // ── Initial load ─────────────────────────────────────────────────

  updateForCurrentPage();
})();
