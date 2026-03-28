# Project 23 — PRIVO Proposed Workflow Diagrams

**Date Received:** March 22, 2026
**From:** Thorn Tayloe (PRIVO)
**Status:** Under Review

---

## Overview

These three sequence diagrams were proposed by PRIVO following our initial integration discussions. They cover the three primary user flows for identity verification and parental consent.

---

## Flow 1: Adult User Identity Verification

**Diagram:** [websequencediagrams link](https://www.websequencediagrams.com/files/render?link=s5ughpAc64Oft6GTjXib7q3ZO3vumPiN5TSyPuc1HbZHme2tn2k3l9KUMVSyHvo0)

**Summary:**
- Adult registers for TrashMob account, then is prompted to verify identity
- TrashMob requests an access token from PRIVO, then submits account details (name, birthdate, email, ext id)
- PRIVO creates a record and returns a ServiceId (SID) + consent identifier + verification URL
- Two options for verification:
  - **Option 1:** Use PRIVO's UI/UX widget (redirect to consent/verification URL)
  - **Option 2:** TrashMob creates a direct verification URL via PRIVO API (POST: Get Identity Verification Widget URL)
- After verification completes, PRIVO sends a webhook with consent status
- TrashMob updates the user record with verification status
- Optional: TrashMob can call GET /UserInfo to retrieve verified data (name, birthdate)

---

## Flow 2: Verified Adult Adds a Child (from Settings)

**Diagram:** [websequencediagrams link](https://www.websequencediagrams.com/files/render?link=wJNZcXdyUWHFqAotQgWiSSALlpnGIbuA6eXSyxjQSze2hL8yLC94eDCNqaejJJa0)

**Summary:**
- Adult (already verified) clicks "Add Child" in settings
- Provides child's birth date, full name, and email address
- TrashMob creates a local record (GUID), then calls PRIVO's Child Consent Request API
  - Passes: child birthdate, full name, email, ext id, parent email (or parent SID)
- PRIVO creates a record and returns child SID + consent identifier + consent URL (not stored)
- Adult is directed through PRIVO's consent flow: review features, approve policies (PP, TOS, Conduct)
- PRIVO sends webhook on consent completion with feature/consent approval status
- TrashMob generates an Azure (Entra) account link, sends account email to child
- Child creates TrashMob account and authenticates
- TrashMob updates the account record linking child to parent

---

## Flow 3: Child-Initiated Account Flow

**Diagram:** [websequencediagrams link](https://www.websequencediagrams.com/files/render?link=twbtelMzto1YDy753PJGGXVbltEYqwFdDxlvtFo43woRkyv4uiwaL5NGbZnzT4ii)

**Summary:**
- Child goes to TrashMob site and enters birthdate (13-17 range)
- System requires parent account — child provides parent email
- TrashMob validates whether parent email exists in the system

**If parent record exists:**
- Child provides first name and email
- "Pending" message shown; TrashMob calls PRIVO Child Consent Request API
- PRIVO sends consent email to parent
- Parent clicks link, reviews features/policies, approves consent
- PRIVO sends webhook on consent completion
- TrashMob generates Azure account link, sends email to child
- Child creates account and authenticates

**If parent record does NOT exist:**
- Child provides first name only
- PRIVO sends email to parent requesting they create a TrashMob account
- Parent follows Adult Account Creation flow (Flow 1), then Adult Adds Child flow (Flow 2)
- Optional: TrashMob can call GET /UserInfo for data retrieval

---

## Review Notes

See analysis in Project 23 planning discussions. Key areas to evaluate:
- Alignment with our Custom Authentication Extension architecture
- Webhook security and reliability
- Account creation sequencing (Entra account vs. TrashMob record)
- PII handling during the consent flow

---

## Related Documents

- [Project 23 — PRIVO API Requirements](./Project_23_Privo_API_Requirements.md) — Detailed 10-section API specification received March 24, 2026
- [Project 23 — Parental Consent for Minors](./Project_23_Parental_Consent.md)
- [Project 23 — Privo Integration Package](./Project_23_Privo_Integration_Package.md)
