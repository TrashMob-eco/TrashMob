Object.defineProperty(exports, "__esModule", { value: true });
exports.Board = void 0;
var React = require("react");
var react_bootstrap_1 = require("react-bootstrap");
var linkedin_svg_1 = require("./assets/card/linkedin.svg");
var JoeBeernink_jpg_1 = require("./assets/boardOfDirectors/JoeBeernink.jpg");
var darylbarber_jpg_1 = require("./assets/boardOfDirectors/darylbarber.jpg");
var KevinGleason_svg_1 = require("./assets/boardOfDirectors/KevinGleason.svg");
var SandraMau_png_1 = require("./assets/boardOfDirectors/SandraMau.png");
var CynthiaMitchell_jpg_1 = require("./assets/boardOfDirectors/CynthiaMitchell.jpg");
var ValerieWilden_svg_1 = require("./assets/boardOfDirectors/ValerieWilden.svg");
var HeroSection_1 = require("./Customization/HeroSection");
var Board = function () {
    React.useEffect(function () {
        window.scrollTo(0, 0);
    });
    return (React.createElement(React.Fragment, null,
        React.createElement(HeroSection_1.HeroSection, { Title: 'Board of Directors', Description: 'Meet our team!' }),
        React.createElement(react_bootstrap_1.Container, { className: 'my-5 pb-5' },
            React.createElement("div", { className: 'p-4 directorCard' },
                React.createElement(react_bootstrap_1.Row, null,
                    React.createElement(react_bootstrap_1.Col, { md: 5 },
                        React.createElement(react_bootstrap_1.Image, { src: JoeBeernink_jpg_1.default, alt: "Joe Beernink", className: "h-100 mt-0 object-fit-cover rounded" })),
                    React.createElement(react_bootstrap_1.Col, { md: 7 },
                        React.createElement("div", { className: 'd-flex justify-content-between align-items-center' },
                            React.createElement("h2", { className: 'm-0 fw-500 font-size-xl color-primary ' }, "Joe Beernink"),
                            React.createElement("a", { href: "https://www.linkedin.com/in/joebeernink/" },
                                React.createElement(react_bootstrap_1.Image, { src: linkedin_svg_1.default, alt: "linkedin icon", className: "h-100 mt-0 object-fit-cover" }))),
                        React.createElement("h5", { className: 'my-3 fw-500 color-grey' }, "Board President"),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Joe Beernink is a software developer with over 25 years of industry experience developing mission-critical software."),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Joe grew up on a small farm in Southern Ontario, Canada, working and playing in the great outdoors, graduated with a degree in Space Science from York University in Toronto in 1994, and moved to the US in 1996. He previously lived in Michigan and Colorado before making Washington State his home in 1999."),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "In 2021, Joe was inspired by Edgar McGregor, a climate activist in California, to get out and start cleaning up his community. After seeing just how much work needed to be done, Joe envisioned a website that enabled like-minded people to get out and start cleaning the environment together, and the idea for TrashMob.eco was born."),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Joe resides in Issaquah, WA with his 2 kids.")))),
            React.createElement("div", { className: 'p-4 directorCard' },
                React.createElement(react_bootstrap_1.Row, null,
                    React.createElement(react_bootstrap_1.Col, { md: 5 },
                        React.createElement(react_bootstrap_1.Image, { src: CynthiaMitchell_jpg_1.default, alt: "Cynthia Mitchell", className: "h-100 mt-0 object-fit-cover rounded" })),
                    React.createElement(react_bootstrap_1.Col, { md: 7 },
                        React.createElement("div", { className: 'd-flex justify-content-between align-items-center' },
                            React.createElement("h2", { className: 'm-0 fw-500 font-size-xl color-primary ' }, "Cynthia Mitchell"),
                            React.createElement("a", { href: "https://www.linkedin.com/in/cynthia-mitchell/" },
                                React.createElement(react_bootstrap_1.Image, { src: linkedin_svg_1.default, alt: "linkedIn icon", className: "h-100 mt-0 object-fit-cover" }))),
                        React.createElement("h5", { className: 'my-3 fw-500 color-grey' }, "Vice President"),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Cynthia Mitchell is a serial tech and media entrepreneur, C-Suite advisor to startups and board member. Mitchell has worked for more than 50 different companies across an array of industries including technology, environmental, energy, construction, health, media, fashion and entertainment. Brands include American Broadcasting Company, Time Warner, Meredith Corporation, Maclean Hunter, Times Mirror, Kaiser Permanente, Mutual of Omaha, and The Summer Olympic Games among others. As a strategist, she has created programs for leading global brands such as Toyota, Nissan, Honda, Coors, Nike, Rolex, and American Express. Over the past several years, she has worked with the Government of India in partnership with The Netherlands to develop biomass plants that recycle agricultural waste into green energy and biofertilizers. She has also led private enterprise initiatives to develop biomass innovation for construction, packaging, displays and other applications."),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Mitchell lives in Southern California. She is the mother of two daughters \u2013 both also entrepreneurs. She is a lifelong equestrian and breeder, a passionate gardener and believes in the collective power of personal service and stewardship to make the world a better, healthier place for all its inhabitants.")))),
            React.createElement("div", { className: 'p-4 directorCard' },
                React.createElement(react_bootstrap_1.Row, null,
                    React.createElement(react_bootstrap_1.Col, { md: 5 },
                        React.createElement(react_bootstrap_1.Image, { src: darylbarber_jpg_1.default, alt: "Daryl Barber", className: "h-100 mt-0 object-fit-cover rounded" })),
                    React.createElement(react_bootstrap_1.Col, { md: 7 },
                        React.createElement("div", { className: 'd-flex justify-content-between align-items-center' },
                            React.createElement("h2", { className: 'm-0 fw-500 font-size-xl color-primary ' }, "Daryl Barber"),
                            React.createElement("a", { href: "https://www.linkedin.com/in/daryl-r-barber-9abb8123/" },
                                React.createElement(react_bootstrap_1.Image, { src: linkedin_svg_1.default, alt: "linkedin icon", className: "h-100 mt-0 object-fit-cover" }))),
                        React.createElement("h5", { className: 'my-3 fw-500 color-grey' }, "Board Treasurer"),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Daryl R. Barber is a Finance Professional with extensive experience at highly complex public and private-equity owned companies within a broad range of businesses, including industrials, chemicals, and software technology and services. Daryl specializes in finance strategies, included in treasury, investor relations, M&A, financial planning & analysis, audit, and corporate and business controllership. Daryl provides consulting services, currently acting as interim Chief Financial Officer and interim Controller and Treasurer for two technology driven companies. "),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "In addition to this finance background, Daryl has experience with several 501(c)(3) organizations, all of which provide assistance to the needy and disadvantaged through education, health, and other human services."),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Having completed his undergraduate studies at the University of Hartford and his graduate studies at Fairleigh Dickinson University, Daryl now resides, with his wife, three children, and a beagle, in Malvern, Pennsylvania.")))),
            React.createElement("div", { className: 'p-4 directorCard' },
                React.createElement(react_bootstrap_1.Row, null,
                    React.createElement(react_bootstrap_1.Col, { md: 5 },
                        React.createElement("a", { href: "https://www.linkedin.com/in/valerie-day-wilden-283a13b5/" },
                            React.createElement(react_bootstrap_1.Image, { src: ValerieWilden_svg_1.default, alt: "Valerie Wilden", className: "h-100 mt-0 object-fit-cover rounded" }))),
                    React.createElement(react_bootstrap_1.Col, { md: 7 },
                        React.createElement("div", { className: 'd-flex justify-content-between align-items-center' },
                            React.createElement("h2", { className: 'm-0 fw-500 font-size-xl color-primary ' }, "Valerie Wilden"),
                            React.createElement(react_bootstrap_1.Image, { src: linkedin_svg_1.default, alt: "linkedIn icon", className: "h-100 mt-0 object-fit-cover" })),
                        React.createElement("h5", { className: 'my-3 fw-500 color-grey' }, "Board Secretary"),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Valerie Wilden is principal consultant for Vivid Communication, the public relations and marketing agency she founded after 35+ years of media and pr, operations, government affairs, crisis management and fundraising for Pennsylvania\u2019s largest nonprofit healthcare organization of its kind. Its uniquely diverse nature also required  Mrs. Wilden to spearhead growth-related communication, planning and volunteer relations for entities that supported its core medical and charitable mission:  a performing arts center, a multitude of outreach programs, special events including renowned VIPS, an auto-repair service, resale shops and the award winning five-state PRESENTS FOR PATIENTS\u00AE program, of which Valerie was a television spokesperson."),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Now at Vivid Communication, she consults with charity and for-profit organizations by writing marketing plans, boosting social media, creating promotions and guiding efforts toward highest net revenue potential. She is a three-term trustee of Westminster College, where she earned her Bachelor of Arts in English. Upon graduating with a Master of Arts in Journalism and Mass Communication from Point Park University, she taught corporate writing there."),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "She and her husband, Greg, live in Wexford, a northern suburb of Pittsburgh, Pennsylvania and are parents of Alyssa, Scott, and Dayne.")))),
            React.createElement("div", { className: 'p-4 directorCard' },
                React.createElement(react_bootstrap_1.Row, null,
                    React.createElement(react_bootstrap_1.Col, { md: 5 },
                        React.createElement(react_bootstrap_1.Image, { src: KevinGleason_svg_1.default, alt: "Kevin Gleason", className: "h-100 mt-0 object-fit-cover rounded" })),
                    React.createElement(react_bootstrap_1.Col, { md: 7 },
                        React.createElement("div", { className: 'd-flex justify-content-between align-items-center' },
                            React.createElement("h2", { className: 'm-0 fw-500 font-size-xl color-primary ' }, "Kevin Gleason"),
                            React.createElement("a", { href: "https://www.linkedin.com/in/kevin-gleason-78a9236/" },
                                React.createElement(react_bootstrap_1.Image, { src: linkedin_svg_1.default, alt: "linkedIn icon", className: "h-100 mt-0 object-fit-cover" }))),
                        React.createElement("h5", { className: 'my-3 fw-500 color-grey' }, "Member at large"),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Kevin Gleason currently is Vice President New York Life and Chief Compliance Officer at MainStay Funds and Index IQ ETFs.\u202F He is a seasoned legal and compliance professional with over 25 years of experience working for 5 Fortune Five Hundred diversified financial services organizations."),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Mr. Gleason has advised and transacted business globally on 5 continents including across Europe, the Middle East, Asia, and South America.\u202F He has counseled C-suite executives and boards of directors on the creation of compliance and ethics programs; the development of controls, training, testing, conflicts identification, and risk assessments; and the structuring of governance frameworks."),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Mr. Gleason has a law degree and a masters in financial services law.\u202F He has earned an MBA from The University of Chicago and BA from University of Notre Dame.\u202F He is or has been a board member at Arizona Science Center, National Society of Compliance Professionals, and Journal of Financial Compliance. He is a frequent speaker at and contributor to industry events and publications.")))),
            React.createElement("div", { className: 'p-4 directorCard' },
                React.createElement(react_bootstrap_1.Row, null,
                    React.createElement(react_bootstrap_1.Col, { md: 5 },
                        React.createElement(react_bootstrap_1.Image, { src: SandraMau_png_1.default, alt: "Sandra Mau", className: "h-100 mt-0 object-fit-cover rounded" })),
                    React.createElement(react_bootstrap_1.Col, { md: 7 },
                        React.createElement("div", { className: 'd-flex justify-content-between align-items-center' },
                            React.createElement("h2", { className: 'm-0 fw-500 font-size-xl color-primary ' }, "Sandra Mau"),
                            React.createElement("a", { href: "https://www.linkedin.com/in/sandramau/" },
                                React.createElement(react_bootstrap_1.Image, { src: linkedin_svg_1.default, alt: "linkedIn icon", className: "h-100 mt-0 object-fit-cover" }))),
                        React.createElement("h5", { className: 'my-3 fw-500 color-grey' }, "Member at large"),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Sandra is VP of Product for Cloud Solutions at Clarivate (NYSE:CLVT). Prior to joining Clarivate via acquisition, she was the CEO and Founder of TrademarkVision, an award-winning AI/Computer Vision startup doing visual brand protection."),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "Sandra is very active in supporting tech and startup communities. She was the Founding Chair of IEEE QLD Women in Engineering, and listed as one of Australia's Top 50 Female Programmers by Pollenizer 2014, and one of Australia's Top 100 Most Influential Engineers by Engineer's Australia 2015. She's also a regular participant in hackathons including past GovHacks and International Women's Day. She was recognised in 2018 by Pittsburgh Business Times with the Pittsburgh Innovator Award and by QUT with the Innovation and Entrepreneurship Outstanding Alumni Award."),
                        React.createElement("p", { className: 'font-size-sm color-grey' }, "She holds a Masters in Robotics from Carnegie Mellon University, a Bachelors in Engineering Science (Aerospace) from University of Toronto, and an MBA from Queensland University of Technology (QUT).")))))));
};
exports.Board = Board;
//# sourceMappingURL=Board.js.map