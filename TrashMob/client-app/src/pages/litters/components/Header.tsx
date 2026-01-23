import { Link } from 'react-router';
import { X } from 'lucide-react';
import { Button } from '@/components/ui/button';

type HeaderProps = {
  leftButton?: React.ReactNode
}
// border-b shadow-md shadow-black/10 bg-white

export const Header = (props: HeaderProps) => {
  return (
    <div className='py-4 fixed w-full'>
      <div className='container'>
        <div className='flex items-center flex-wrap flex-row'>
          {props.leftButton 
          ? props.leftButton : (
            <Button variant='ghost' size="icon" asChild>
              <Link to="/"><X /></Link>
            </Button>
          )}
        </div>
      </div>
    </div>
  )
}