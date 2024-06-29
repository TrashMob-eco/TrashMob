"use strict";
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var react_router_dom_1 = require("react-router-dom");
var AuthStore_1 = require("../store/AuthStore");
var logo_svg_1 = require("./assets/logo.svg");
var react_bootstrap_1 = require("react-bootstrap");
require("./assets/styles/header.css");
var react_bootstrap_icons_1 = require("react-bootstrap-icons");
var TopMenu = function (props) {
    var _a = React.useState(""), userName = _a[0], setUserName = _a[1];
    var _b = React.useState(props.isUserLoaded), isUserLoaded = _b[0], setIsUserLoaded = _b[1];
    React.useEffect(function () {
        if (props.currentUser && props.isUserLoaded) {
            setUserName(props.currentUser.userName);
        }
        setIsUserLoaded(props.isUserLoaded);
    }, [props.currentUser, props.isUserLoaded]);
    var mainNavItems = __spreadArray(__spreadArray([
        { name: "Getting Started", url: "/gettingstarted" }
    ], isUserLoaded ? [{ name: "My Dashboard", url: "/mydashboard" }] : [], true), [
        { name: "Events", url: "/#events" },
        { name: "Donate", url: "https://donate.stripe.com/14k9DN2EnfAog9O3cc" },
        { name: "Shop", url: "/shop" }
    ], false);
    function signOut(e) {
        e.preventDefault();
        var logoutRequest = {
            account: AuthStore_1.msalClient.getActiveAccount(),
        };
        AuthStore_1.msalClient.logout(logoutRequest);
    }
    function profileEdit(e) {
        e.preventDefault();
        var account = AuthStore_1.msalClient.getAllAccounts()[0];
        var policy = (0, AuthStore_1.getB2CPolicies)();
        var scopes = (0, AuthStore_1.getApiConfig)();
        var request = {
            account: account,
            authority: policy.authorities.profileEdit.authority,
            scopes: scopes.b2cScopes,
        };
        AuthStore_1.msalClient.acquireTokenRedirect(request);
    }
    function signIn(e) {
        e.preventDefault();
        var apiConfig = (0, AuthStore_1.getApiConfig)();
        AuthStore_1.msalClient.loginRedirect({
            scopes: apiConfig.b2cScopes
        });
    }
    return (React.createElement("header", { className: "tm-header" },
        React.createElement("div", { className: "container bg-light tm-mainNav" },
            React.createElement("div", { className: "navbar navbar-expand-lg navbar-light navbar-static-top px-0", id: "navbar" },
                React.createElement("a", { className: "navbar-brand", href: "/", id: "navbarBrand" },
                    React.createElement("img", { src: logo_svg_1.default, alt: "TrashMob Logo", className: "logo m-0" })),
                React.createElement("button", { className: "navbar-toggler", type: "button", "data-toggle": "collapse", "data-target": "#navbarNav", "aria-controls": "navbarNav", "aria-expanded": "false", "aria-label": "Toggle navigation" },
                    React.createElement("span", { className: "navbar-toggler-icon" })),
                React.createElement("div", { className: "navbar-collapse collapse", id: "navbarNav" },
                    React.createElement("ul", { className: "nav navbar-nav" }, mainNavItems.map(function (item) { return (React.createElement("li", { key: item.name },
                        React.createElement(react_bootstrap_1.Nav.Link, { href: item.url }, item.name))); })),
                    React.createElement(react_bootstrap_1.Button, { hidden: isUserLoaded, className: "btn btn-primary", onClick: function (e) { return signIn(e); }, id: "loginBtn" }, "Sign in"),
                    React.createElement(react_bootstrap_1.Dropdown, { hidden: !isUserLoaded },
                        React.createElement(react_bootstrap_1.Dropdown.Toggle, { id: "userBtn", variant: "light" },
                            React.createElement(react_bootstrap_icons_1.PersonCircle, { className: "mr-3", size: 32, color: "#96ba00", "aria-labelledby": "userName" }),
                            userName ? userName : 'Welcome'),
                        React.createElement(react_bootstrap_1.Dropdown.Menu, { className: "shadow border-0" },
                            React.createElement(react_bootstrap_1.Dropdown.Item, { eventKey: "1", href: "/mydashboard" },
                                React.createElement(react_bootstrap_icons_1.Speedometer2, { "aria-hidden": "true" }),
                                "My dashboard"),
                            React.createElement(react_bootstrap_1.Dropdown.Divider, null),
                            React.createElement(react_bootstrap_1.Dropdown.Item, { eventKey: "2", href: "/manageeventdashboard" },
                                React.createElement(react_bootstrap_icons_1.PlusLg, { "aria-hidden": "true" }),
                                "Add event"),
                            React.createElement(react_bootstrap_1.Dropdown.Divider, null),
                            React.createElement(react_bootstrap_1.Dropdown.Item, { eventKey: "3", onClick: function (e) { return profileEdit(e); } },
                                React.createElement(react_bootstrap_icons_1.Person, { "aria-hidden": "true" }),
                                "Update my profile"),
                            React.createElement(react_bootstrap_1.Dropdown.Divider, null),
                            React.createElement(react_bootstrap_1.Dropdown.Item, { eventKey: "4", href: "/locationpreference" },
                                React.createElement(react_bootstrap_icons_1.Map, { "aria-hidden": "true" }),
                                "My location preference"),
                            React.createElement(react_bootstrap_1.Dropdown.Divider, null),
                            props.currentUser.isSiteAdmin ? React.createElement(React.Fragment, null,
                                " ",
                                React.createElement(react_bootstrap_1.Dropdown.Item, { eventKey: "5", href: "/siteadmin", disabled: !props.currentUser.isSiteAdmin },
                                    React.createElement(react_bootstrap_icons_1.PersonBadge, { "aria-hidden": "true" }),
                                    "Site administration"),
                                React.createElement(react_bootstrap_1.Dropdown.Divider, null)) : "",
                            React.createElement(react_bootstrap_1.Dropdown.Item, { eventKey: "6", href: "/deletemydata" },
                                React.createElement(react_bootstrap_icons_1.PersonX, { "aria-hidden": "true" }),
                                "Delete my account"),
                            React.createElement(react_bootstrap_1.Dropdown.Divider, null),
                            React.createElement(react_bootstrap_1.Dropdown.Item, { eventKey: "7", onClick: function (e) { return signOut(e); } },
                                React.createElement(react_bootstrap_icons_1.BoxArrowLeft, { "aria-hidden": "true" }),
                                "Sign out"))),
                    React.createElement(react_bootstrap_1.Button, { className: "btn", href: "/help", id: "helpBtn" },
                        React.createElement(react_bootstrap_icons_1.QuestionCircle, null)))))));
};
exports.default = (0, react_router_dom_1.withRouter)(TopMenu);
//# sourceMappingURL=TopMenu.js.map