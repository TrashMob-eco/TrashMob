"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var react_router_dom_1 = require("react-router-dom");
var PartnerEdit_1 = require("./PartnerEdit");
var PartnerAdmins_1 = require("./PartnerAdmins");
var PartnerLocations_1 = require("./PartnerLocations");
var react_bootstrap_1 = require("react-bootstrap");
var guid_typescript_1 = require("guid-typescript");
var PartnerDocuments_1 = require("./PartnerDocuments");
var PartnerSocialMediaAccounts_1 = require("./PartnerSocialMediaAccounts");
var PartnerContacts_1 = require("./PartnerContacts");
var PartnerDashboard = function (props) {
    var _a;
    var _b = React.useState('1'), radioValue = _b[0], setRadioValue = _b[1];
    var _c = React.useState(""), partnerId = _c[0], setPartnerId = _c[1];
    var _d = React.useState(), isPartnerIdReady = _d[0], setIsPartnerIdReady = _d[1];
    var _e = React.useState((_a = props.match) === null || _a === void 0 ? void 0 : _a.params["partnerId"]), loadedPartnerId = _e[0], setLoadedPartnerId = _e[1];
    var radios = [
        { name: 'Manage Partner', value: '1' },
        { name: 'Manage Partner Locations', value: '2' },
        { name: 'Manage Partner Contacts', value: '3' },
        { name: 'Manage Partner Admins', value: '4' },
        { name: 'Manage Partner Documents', value: '5' },
        { name: 'Manage Partner Social Media Accounts', value: '6' },
    ];
    React.useEffect(function () {
        var partId = loadedPartnerId;
        if (!partId) {
            setPartnerId(guid_typescript_1.Guid.createEmpty().toString());
            setLoadedPartnerId(guid_typescript_1.Guid.createEmpty().toString());
        }
        else {
            setPartnerId(partId);
        }
        setIsPartnerIdReady(true);
    }, [loadedPartnerId]);
    function renderEditPartner() {
        return (React.createElement("div", null,
            React.createElement(PartnerEdit_1.PartnerEdit, { partnerId: partnerId, currentUser: props.currentUser, isUserLoaded: props.isUserLoaded })));
    }
    function renderPartnerAdmins() {
        return (React.createElement("div", null,
            React.createElement(PartnerAdmins_1.PartnerAdmins, { partnerId: partnerId, currentUser: props.currentUser, isUserLoaded: props.isUserLoaded })));
    }
    function renderPartnerLocations() {
        return (React.createElement("div", null,
            React.createElement(PartnerLocations_1.PartnerLocations, { partnerId: partnerId, currentUser: props.currentUser, isUserLoaded: props.isUserLoaded })));
    }
    function renderPartnerContacts() {
        return (React.createElement("div", null,
            React.createElement(PartnerContacts_1.PartnerContacts, { partnerId: partnerId, currentUser: props.currentUser, isUserLoaded: props.isUserLoaded })));
    }
    function renderPartnerDocuments() {
        return (React.createElement("div", null,
            React.createElement(PartnerDocuments_1.PartnerDocuments, { partnerId: partnerId, currentUser: props.currentUser, isUserLoaded: props.isUserLoaded })));
    }
    function renderPartnerSocialMediaAccounts() {
        return (React.createElement("div", null,
            React.createElement(PartnerSocialMediaAccounts_1.PartnerSocialMediaAccounts, { partnerId: partnerId, currentUser: props.currentUser, isUserLoaded: props.isUserLoaded })));
    }
    function renderPartnerDashboard() {
        return (React.createElement(react_bootstrap_1.Container, null,
            React.createElement(react_bootstrap_1.Col, null,
                React.createElement(react_bootstrap_1.Row, { className: "gx-2 py-5" },
                    React.createElement("div", { className: "bg-white p-5 shadow-sm rounded" },
                        React.createElement(react_bootstrap_1.ButtonGroup, null, radios.map(function (radio, idx) { return (React.createElement(react_bootstrap_1.ToggleButton, { key: idx, id: "radio-".concat(idx), type: "radio", variant: idx % 2 ? 'outline-success' : 'outline-danger', name: "radio", value: radio.value, checked: radioValue === radio.value, onChange: function (e) { return setRadioValue(e.currentTarget.value); } }, radio.name)); })),
                        radioValue === '1' && renderEditPartner(),
                        radioValue === '2' && renderPartnerLocations(),
                        radioValue === '3' && renderPartnerContacts(),
                        radioValue === '4' && renderPartnerAdmins(),
                        radioValue === '5' && renderPartnerDocuments(),
                        radioValue === '6' && renderPartnerSocialMediaAccounts())))));
    }
    var contents = isPartnerIdReady
        ? renderPartnerDashboard()
        : React.createElement("p", null,
            React.createElement("em", null, "Loading..."));
    return React.createElement("div", null,
        React.createElement("hr", null),
        contents);
};
exports.default = (0, react_router_dom_1.withRouter)(PartnerDashboard);
//# sourceMappingURL=PartnerDashboard.js.map