import { Link } from 'react-router-dom';
import { cn } from "@/lib/utils"
import logo from '../assets/logowhite.svg';

const navClassName = "text-white text-[13px] uppercase font-medium"
const socialClassName = "w-6 h-6 bg-white !rounded-lg flex justify-center items-center"

export const SiteFooter = () => {
	const footerNavs = [
		{ href: '/aboutus', title: 'About us' },
		{ href: '/board', title: 'Board' },
		{ href: '/partnerships', title: 'Partnerships' },
		{ href: '/faq', title: 'FAQ' },
		{ href: '/volunteeropportunities', title: 'Volunteer opportunities' },
		{ href: '/contactus', title: 'Contact us' },
		{ href: 'https://donate.stripe.com/14k9DN2EnfAog9O3cc', title: 'Donate' },
		{ href: '/privacypolicy', title: 'Privacy policy', className: "row-start-1" },
		{ href: '/termsofservice', title: 'Terms of service' },
		{ href: '/waivers', title: 'Waivers' },
		{ href: '/eventsummaries', title: 'Event Summaries' },
	]

  	return (
	  	<footer className="tailwind">
			<div className="bg-[#212529] !py-10">
				<div className="container">
					<div>
						<img src={logo} alt='TrashMob Logo' id='logo_footer' />
					</div>
					<div className="grid grid-rows-12 grid-cols-1 md:grid-rows-4 md:grid-cols-3 gap-4 gap-x-8 md:grid-flow-col">
						{footerNavs.map(nav => {
							return nav.href.startsWith('/') 
								? <Link to={nav.href} key={nav.href} className={cn(navClassName, nav.className)}>{nav.title}</Link> 
								: <a href={nav.href} className={cn(navClassName, nav.className)}>{nav.title}</a>
						})}
					</div>
					<div className="border-t border-white !mt-8 md:!mt-12 text-white">
						<div className="flex flex-col md:flex-row gap-8">
							<div className="basis-3/4">
								<div className="text-[13px] font-[Montserrat] font-serif font-medium">
									<p>Copyright &copy; 2023 TRASHMOB.ECO - All rights reserved.</p>
									<p>TrashMob is a non-profit, 501(c)(3) organization based in Washington State, USA.</p>
									<p>US Federal Employer Id (EIN): 88-1286026</p>
								</div>
							</div>
							<div className="basis-1/4">
								<div className="flex flex-row justify-start md:justify-end !mt-4 gap-2">
									<a
										href="https://www.facebook.com/trashmob.eco/"
										target="_blank"
										rel="noreferrer noopener"
										className={socialClassName}
									>
										<i className="!mt-0 fab fa-facebook-f" />
									</a>
									<a
										href="https://twitter.com/TrashMobEco"
										target="_blank"
										rel="noreferrer noopener"
										className={socialClassName}
									>
										<i className='!mt-0 fab fa-twitter' />
									</a>
									<a
										href="https://profiles.eco/trashmob?ref=tm"
										target="_blank"
										rel="noreferrer noopener"
										className={socialClassName}
									>
										<img
											className='eco-trustmark'
											alt='.eco profile for trashmob.eco'
											src='https://trust.profiles.eco/trashmob/eco-button.svg?color=%23000000'
										/>
									</a>
									<a
										href="https://www.instagram.com/trashmobinfo"
										target="_blank"
										rel="noreferrer noopener"
										className={socialClassName}
									>
										<i className='!mt-0 fa-brands fa-instagram' />
									</a>
									<a
										href="https://www.youtube.com/channel/UC2LgFmXFCA8kdkxd4IJ51BA"
										target="_blank"
										rel="noreferrer noopener"
										className={socialClassName}
									>
										<i className='!mt-0 fa-brands fa-youtube' />
									</a>
									<a
										href="https://www.linkedin.com/company/76188984"
										target="_blank"
										rel="noreferrer noopener"
										className={socialClassName}
									>
										<i className='!mt-0 fa-brands fa-linkedin' />
									</a>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</footer>
  	) 
}