"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var react_router_dom_1 = require("react-router-dom");
var react_bootstrap_1 = require("react-bootstrap");
var guid_typescript_1 = require("guid-typescript");
var react_query_1 = require("@tanstack/react-query");
var EditEvent_1 = require("./EditEvent");
var ManageEventPartners_1 = require("./ManageEventPartners");
var ManageEventAttendees_1 = require("./ManageEventAttendees");
var HeroSection_1 = require("../Customization/HeroSection");
var events_1 = require("../../services/events");
var services_config_1 = require("../../config/services.config");
var ManageEventDashboard = function (props) {
    var _a;
    var _b = React.useState(''), eventId = _b[0], setEventId = _b[1];
    var _c = React.useState(), isEventIdReady = _c[0], setIsEventIdReady = _c[1];
    var _d = React.useState((_a = props.match) === null || _a === void 0 ? void 0 : _a.params.eventId), loadedEventId = _d[0], setLoadedEventId = _d[1];
    var _e = React.useState(false), isEventComplete = _e[0], setIsEventComplete = _e[1];
    var getEventById = (0, react_query_1.useQuery)({
        queryKey: (0, events_1.GetEventById)({ eventId: eventId }).key,
        queryFn: (0, events_1.GetEventById)({ eventId: eventId }).service,
        staleTime: services_config_1.Services.CACHE.DISABLE,
        enabled: false,
    });
    React.useEffect(function () {
        var evId = loadedEventId;
        if (!evId) {
            setEventId(guid_typescript_1.Guid.createEmpty().toString());
            setLoadedEventId(guid_typescript_1.Guid.createEmpty().toString());
        }
        else if (evId !== guid_typescript_1.Guid.EMPTY) {
            setEventId(evId);
            // Check to see if this event has been completed
            getEventById.refetch().then(function (res) {
                var _a;
                if (res.data !== undefined && new Date((_a = res.data) === null || _a === void 0 ? void 0 : _a.data.eventDate) < new Date())
                    setIsEventComplete(true);
            });
        }
        setIsEventIdReady(true);
    }, [loadedEventId]);
    function handleEditCancel() {
        props.history.push('/mydashboard');
    }
    function handleEditSave() {
        props.history.push({
            pathname: '/mydashboard',
            state: {
                newEventCreated: true,
            },
        });
    }
    function renderEventDashboard() {
        return (React.createElement(React.Fragment, null,
            React.createElement(EditEvent_1.EditEvent, { eventId: eventId, currentUser: props.currentUser, isUserLoaded: props.isUserLoaded, onEditCancel: handleEditCancel, onEditSave: handleEditSave, history: props.history, location: props.location, match: props.match }),
            React.createElement(ManageEventAttendees_1.ManageEventAttendees, { eventId: eventId, currentUser: props.currentUser, isUserLoaded: props.isUserLoaded }),
            React.createElement(ManageEventPartners_1.ManageEventPartners, { eventId: eventId, currentUser: props.currentUser, isUserLoaded: props.isUserLoaded, isEventComplete: isEventComplete })));
    }
    var contents = isEventIdReady ? (renderEventDashboard()) : (React.createElement("p", null,
        React.createElement("em", null, "Loading...")));
    return (React.createElement("div", null,
        React.createElement(HeroSection_1.HeroSection, { Title: 'Manage Event', Description: 'We can\u2019t wait to see the results.' }),
        React.createElement(react_bootstrap_1.Container, null,
            React.createElement(react_bootstrap_1.Row, { className: 'gx-2 py-5', lg: 2 },
                React.createElement(react_bootstrap_1.Col, { lg: 4, className: 'd-flex' },
                    React.createElement("div", { className: 'bg-white py-2 px-5 shadow-sm rounded' },
                        React.createElement("h2", { className: 'color-primary mt-4 mb-5' }, "Manage Event"),
                        React.createElement("p", null, "This page allows you to create a new event or edit an existing event. You can set the name, time, and location for the event, and then request services from TrashMob.eco Partners."))),
                React.createElement(react_bootstrap_1.Col, { lg: 8 },
                    React.createElement("div", { className: 'bg-white p-5 shadow-sm rounded' }, contents))))));
};
exports.default = (0, react_router_dom_1.withRouter)(ManageEventDashboard);
//# sourceMappingURL=ManageEventDashboard.js.map