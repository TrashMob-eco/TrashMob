"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var react_bootstrap_1 = require("react-bootstrap");
var react_router_dom_1 = require("react-router-dom");
var AdminEmailTemplates_1 = require("./AdminEmailTemplates");
var AdminEvents_1 = require("./AdminEvents");
var AdminPartnerRequests_1 = require("./AdminPartnerRequests");
var AdminPartners_1 = require("./AdminPartners");
var AdminSendNotifications_1 = require("./AdminSendNotifications");
var AdminUsers_1 = require("./AdminUsers");
var SiteAdmin = function (props) {
    var _a = React.useState(props.currentUser), currentUser = _a[0], setCurrentUser = _a[1];
    var _b = React.useState(props.isUserLoaded), isUserLoaded = _b[0], setIsUserLoaded = _b[1];
    var _c = React.useState(false), isSiteAdmin = _c[0], setIsSiteAdmin = _c[1];
    var _d = React.useState('1'), radioValue = _d[0], setRadioValue = _d[1];
    var radios = [
        { name: 'Manage Users', value: '1' },
        { name: 'Manage Events', value: '2' },
        { name: 'Manage Partners', value: '3' },
        { name: 'Manage Partner Requests', value: '4' },
        { name: 'View Executive Summary', value: '5' },
        { name: 'Send Notifications', value: '6' },
        { name: 'View Email Templates', value: '7' },
    ];
    React.useEffect(function () {
        setIsSiteAdmin(props.currentUser.isSiteAdmin);
        setCurrentUser(props.currentUser);
        setIsUserLoaded(props.isUserLoaded);
    }, [props.currentUser, props.currentUser.isSiteAdmin, props.isUserLoaded]);
    function renderManageEvents() {
        return (React.createElement("div", null,
            React.createElement(AdminEvents_1.AdminEvents, { history: props.history, location: props.location, match: props.match, currentUser: currentUser, isUserLoaded: isUserLoaded })));
    }
    function renderManageUsers() {
        return (React.createElement("div", null,
            React.createElement(AdminUsers_1.AdminUsers, { history: props.history, location: props.location, match: props.match, currentUser: currentUser, isUserLoaded: isUserLoaded })));
    }
    function renderManagePartners() {
        return (React.createElement("div", null,
            React.createElement(AdminPartners_1.AdminPartners, { history: props.history, location: props.location, match: props.match, currentUser: currentUser, isUserLoaded: isUserLoaded })));
    }
    function renderManagePartnerRequests() {
        return (React.createElement("div", null,
            React.createElement(AdminPartnerRequests_1.AdminPartnerRequests, { history: props.history, location: props.location, match: props.match, currentUser: currentUser, isUserLoaded: isUserLoaded })));
    }
    function renderSendNotifications() {
        return (React.createElement("div", null,
            React.createElement(AdminSendNotifications_1.AdminSendNotifications, { history: props.history, location: props.location, match: props.match, currentUser: currentUser, isUserLoaded: isUserLoaded })));
    }
    function renderEmailTemplates() {
        return (React.createElement("div", null,
            React.createElement(AdminEmailTemplates_1.AdminEmailTemplates, { history: props.history, location: props.location, match: props.match, currentUser: currentUser, isUserLoaded: isUserLoaded })));
    }
    function renderExecutiveSummary() {
        return (React.createElement("div", null, "Executive Summary"));
    }
    function renderAdminTable() {
        return (React.createElement("div", null,
            React.createElement(react_bootstrap_1.ButtonGroup, null, radios.map(function (radio, idx) { return (React.createElement(react_bootstrap_1.ToggleButton, { key: idx, id: "radio-".concat(idx), type: "radio", variant: idx % 2 ? 'outline-success' : 'outline-danger', name: "radio", value: radio.value, checked: radioValue === radio.value, onChange: function (e) { return setRadioValue(e.currentTarget.value); } }, radio.name)); })),
            radioValue === '1' && renderManageUsers(),
            radioValue === '2' && renderManageEvents(),
            radioValue === '3' && renderManagePartners(),
            radioValue === '4' && renderManagePartnerRequests(),
            radioValue === '5' && renderExecutiveSummary(),
            radioValue === '6' && renderSendNotifications(),
            radioValue === '7' && renderEmailTemplates()));
    }
    return (React.createElement(react_bootstrap_1.Container, null,
        React.createElement("h1", { className: 'font-weight-bold' }, "Site Administration"),
        React.createElement(react_bootstrap_1.Row, { className: "gx-2 py-5", lg: 2 },
            React.createElement(react_bootstrap_1.Col, { lg: 12 },
                React.createElement("div", null,
                    !isSiteAdmin && React.createElement("p", null,
                        React.createElement("em", null, "Access Denied")),
                    isSiteAdmin && renderAdminTable())))));
};
exports.default = (0, react_router_dom_1.withRouter)(SiteAdmin);
//# sourceMappingURL=SiteAdmin.js.map