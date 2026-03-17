import json

article = {
    "data": {
        "title": "What Your Community Page Actually Does: A Tour of TrashMob\u2019s New Tools for Program Managers",
        "slug": "what-your-community-page-actually-does",
        "excerpt": "A closer look at the tools TrashMob gives community program managers \u2014 branded pages, regional support, impact dashboards, volunteer engagement, and the enrollment process that gets you up and running.",
        "author": "Joe Beernink",
        "isFeatured": True,
        "estimatedReadTime": 5,
        "tags": ["communities", "features", "program-management", "analytics", "volunteers"],
        "category": {"connect": [1]},
        "body": []
    }
}

body = article["data"]["body"]

def p(text):
    return {"type": "paragraph", "children": [{"type": "text", "text": text}]}

def h3(text):
    return {"type": "heading", "level": 3, "children": [{"type": "text", "text": text}]}

def bold_para(bold_text, rest_text):
    return {"type": "paragraph", "children": [
        {"type": "text", "bold": True, "text": bold_text},
        {"type": "text", "text": rest_text}
    ]}

def link_para(parts):
    children = []
    for part in parts:
        if "link" in part:
            children.append({"type": "link", "url": part["link"], "children": [{"type": "text", "text": part["text"]}]})
        elif "bold" in part:
            children.append({"type": "text", "bold": True, "text": part["text"]})
        elif "italic" in part:
            children.append({"type": "text", "italic": True, "text": part["text"]})
        else:
            children.append({"type": "text", "text": part["text"]})
    return {"type": "paragraph", "children": children}

body.append(p("Last week we announced the 2026 platform launch. This week, let\u2019s get specific about what the community tools actually look like and how they work for program managers running cleanup initiatives at the city, county, or state level."))

body.append(h3("Your community, your brand"))
body.append(p("Every TrashMob community gets a dedicated public page at trashmob.eco/communities/your-community. It\u2019s not a generic profile \u2014 it\u2019s a branded landing page that represents your organization."))
body.append(p("You control the logo, banner image, brand colors, tagline, and description. When volunteers visit your page, they see your identity \u2014 not ours. Your events, teams, litter reports, and impact stats all roll up under your community automatically."))
body.append(p("The admin dashboard gives you a content editor where you can update all of this in minutes. Change your banner for a seasonal campaign, update your description for a new initiative \u2014 no tickets to file, no waiting on anyone."))

body.append(h3("City, county, or statewide \u2014 we support your geography"))
body.append(p("Not every cleanup program operates at the city level. County road commissions, state DOTs, regional conservation districts, watershed authorities \u2014 they all manage cleanup programs across larger geographies."))
body.append(p("TrashMob supports city, county, state, and custom regional community types. Each community defines its coverage area with a geographic boundary, and the platform automatically scopes events, teams, and litter reports to that area. The map on your community page zooms to fit your region, whether that\u2019s a small town or an entire state."))
body.append(p("This means a county-level community sees everything happening across all its cities. A state DOT sees activity along every highway corridor in its jurisdiction. The data rolls up to match how your organization actually operates."))

body.append(h3("Impact numbers that are ready when the board asks"))
body.append(p("Every event in your community captures structured data: number of volunteers, bags collected, weight removed, hours worked, GPS route traces, and before-and-after photos. That data flows into your community dashboard automatically."))
body.append(p("Your dashboard shows total events, total participants, bags collected, and weight removed \u2014 filterable by date range. When your city council, board of directors, or grant funder asks \u201cwhat have you accomplished this year?\u201d the answer is a few clicks away, not a week of digging through spreadsheets."))
body.append(p("The data is exportable. Pull a CSV for your annual report. Show a heat map of litter concentration to justify where to focus resources next quarter. Use the photo gallery in a presentation to your funders."))

body.append(h3("Keeping volunteers engaged after the first cleanup"))
body.append(p("Getting volunteers to show up once is easy. Getting them to come back is the hard part. TrashMob gives you multiple tools to keep your community engaged:"))
body.append(bold_para("Teams", " let volunteers organize with friends, coworkers, or neighbors. Teams have their own identity, leaderboard ranking, and collective impact stats. When people clean up as a group, they stay motivated longer."))
body.append(bold_para("Community leaderboards", " recognize individual and team contributions within your community. Friendly competition drives repeat participation \u2014 people want to see their name climb the rankings."))
body.append(bold_para("Bulk email invites", " let you upload a CSV of contacts and send targeted invitations to join your community. Onboard a neighborhood association, a corporate partner\u2019s employee group, or your existing volunteer list in one batch."))
body.append(bold_para("Newsletters", " keep your community in the loop with event announcements, milestone celebrations, and impact updates. Compose and send directly from your admin dashboard."))

body.append(h3("Coming soon: Adopt-a-Location"))
body.append(p("We\u2019re putting the finishing touches on our adopt-a-location system \u2014 interactive maps where volunteers and teams claim specific streets, parks, trails, and waterways for ongoing maintenance. It\u2019s a big feature, and it deserves its own deep dive. Stay tuned for the next blog post where we\u2019ll walk through the full adoption workflow, sponsored adoptions for businesses, and the AI-powered tools that help you set up your adoptable areas."))

body.append(h3("Getting started takes five minutes"))
body.append(p("The enrollment process is straightforward:"))
body.append(p("1. Visit the \u201cFor Communities\u201d page at trashmob.eco"))
body.append(p("2. Click \u201cStart Your Community\u201d and fill out the partner request form \u2014 select \u201cCommunity\u201d as your type, tell us about your organization and coverage area"))
body.append(p("3. Our team reviews your application (typically within a few business days)"))
body.append(p("4. We set up your community page, configure your geographic boundary, and hand you the keys to the admin dashboard"))
body.append(p("From there, you customize your branding, invite your volunteers, and start creating events. Most communities are fully operational within a week of approval."))

body.append(h3("Free for volunteers, always"))
body.append(p("One thing we hear consistently from program managers: \u201cI can\u2019t ask my volunteers to pay for another app.\u201d You don\u2019t have to. Individual volunteers use TrashMob completely free \u2014 join events, track personal impact, report litter, join teams. That never changes."))
body.append(p("Community management tools are what we offer to organizations, and we work with each community to find a partnership model that fits their budget and needs. We\u2019re a 501(c)(3) nonprofit, and our goal is adoption, not revenue maximization."))

body.append(h3("Let\u2019s get your community set up"))
body.append(p("If you run a cleanup program and you\u2019re still coordinating through email, spreadsheets, and paper waivers \u2014 we built this for you."))
body.append(link_para([
    {"bold": True, "text": "Visit "},
    {"link": "https://www.trashmob.eco/for-communities", "text": "trashmob.eco/for-communities"},
    {"bold": True, "text": " to learn more, or email "},
    {"link": "mailto:info@trashmob.eco", "text": "info@trashmob.eco"},
    {"bold": True, "text": " to schedule a walkthrough."}
]))
body.append(link_para([{"italic": True, "text": "TrashMob.eco is a 501(c)(3) nonprofit dedicated to empowering communities to keep their neighborhoods clean."}]))

with open("D:/repos/TrashMob2/Planning/NewsArticles/community-features-upload.json", "w", encoding="utf-8") as f:
    json.dump(article, f, ensure_ascii=False, indent=2)

print(f"Generated {len(body)} blocks")
