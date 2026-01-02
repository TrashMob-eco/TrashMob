import { Link } from 'react-router';
import { Button } from '../ui/button';
import { X } from 'lucide-react';

export const ModalSiteHeader = () => (
  <div className='border-b shadow-md shadow-black/10 bg-white py-4 fixed w-full'>
		<div className='container'>
			<div className='flex items-center flex-wrap flex-row'>
				<Button variant='ghost' size="icon" asChild>
					<Link to="/"><X /></Link>
				</Button>
			</div>
		</div>
	</div>
)