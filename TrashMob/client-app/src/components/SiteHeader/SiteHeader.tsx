import {
  NavigationMenu,
  NavigationMenuContent,
  NavigationMenuIndicator,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
  NavigationMenuTrigger,
  NavigationMenuViewport,
} from "@/components/ui/navigation-menu"
import { NavLink } from 'react-router-dom'
import logo from '../assets/TrashMob_Logo1.png';

export const SiteHeader = () => {
  return (
    <div className="flex-row">
      <a className="flex w-[230px]" href='/'>
          <img src={logo} alt='TrashMob Logo' className='logo' />
      </a>
      <button
          className='navbar-toggler p-2'
          type='button'
          data-toggle='collapse'
          data-target='#navbarNav'
          aria-controls='navbarNav'
          aria-expanded='false'
          aria-label='Toggle navigation'
      >
          <span className='navbar-toggler-icon' />
      </button>
      <NavigationMenu>
        <NavigationMenuList>
          <NavigationMenuItem>
            <NavLink
              to="/gettingstart"
              className={({ isActive }) => {
                return isActive ? 'text-red-400' : 'text-black';
              }}
            >
              <NavigationMenuLink>
                Getting Start
              </NavigationMenuLink>
            </NavLink>
          </NavigationMenuItem>
        </NavigationMenuList>
      </NavigationMenu>
    </div>
  )
}