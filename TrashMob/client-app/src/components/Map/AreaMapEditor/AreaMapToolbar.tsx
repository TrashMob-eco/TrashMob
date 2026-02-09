import { Pentagon, Route, Pencil, Trash2 } from 'lucide-react';
import { Button } from '@/components/ui/button';

export type DrawingMode = 'polygon' | 'polyline' | 'edit' | null;

interface AreaMapToolbarProps {
    activeMode: DrawingMode;
    hasShape: boolean;
    onModeChange: (mode: DrawingMode) => void;
    onDelete: () => void;
}

export const AreaMapToolbar = ({ activeMode, hasShape, onModeChange, onDelete }: AreaMapToolbarProps) => {
    const toggle = (mode: 'polygon' | 'polyline' | 'edit') => {
        onModeChange(activeMode === mode ? null : mode);
    };

    return (
        <div className='flex gap-1 p-2 bg-muted rounded-t-lg'>
            <Button
                type='button'
                variant='outline'
                size='sm'
                disabled={hasShape}
                className={
                    activeMode === 'polygon'
                        ? 'bg-primary text-primary-foreground hover:bg-primary/90 hover:text-primary-foreground'
                        : ''
                }
                onClick={() => toggle('polygon')}
                title='Draw polygon'
            >
                <Pentagon className='h-4 w-4 mr-1' />
                Polygon
            </Button>
            <Button
                type='button'
                variant='outline'
                size='sm'
                disabled={hasShape}
                className={
                    activeMode === 'polyline'
                        ? 'bg-primary text-primary-foreground hover:bg-primary/90 hover:text-primary-foreground'
                        : ''
                }
                onClick={() => toggle('polyline')}
                title='Draw line'
            >
                <Route className='h-4 w-4 mr-1' />
                Line
            </Button>
            <Button
                type='button'
                variant='outline'
                size='sm'
                disabled={!hasShape}
                className={
                    activeMode === 'edit'
                        ? 'bg-primary text-primary-foreground hover:bg-primary/90 hover:text-primary-foreground'
                        : ''
                }
                onClick={() => toggle('edit')}
                title='Edit vertices'
            >
                <Pencil className='h-4 w-4 mr-1' />
                Edit
            </Button>
            <Button
                type='button'
                variant='outline'
                size='sm'
                disabled={!hasShape}
                onClick={onDelete}
                title='Delete shape'
            >
                <Trash2 className='h-4 w-4 mr-1' />
                Delete
            </Button>
        </div>
    );
};
