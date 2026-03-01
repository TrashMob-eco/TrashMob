import { useEffect, useMemo, useState } from 'react';
import { Search } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Badge } from '@/components/ui/badge';
import { GuideEntryCollapsible } from './guide-section';
import { adminGuideData } from './guide-data';

export const SiteAdminGuide = () => {
    const [searchQuery, setSearchQuery] = useState('');
    const [activeTab, setActiveTab] = useState(adminGuideData[0].id);

    const filteredData = useMemo(() => {
        if (!searchQuery.trim()) return adminGuideData;
        const q = searchQuery.toLowerCase();
        return adminGuideData
            .map((section) => ({
                ...section,
                entries: section.entries.filter(
                    (entry) =>
                        entry.title.toLowerCase().includes(q) ||
                        entry.content.toLowerCase().includes(q) ||
                        entry.keywords?.some((k) => k.toLowerCase().includes(q)),
                ),
            }))
            .filter((section) => section.entries.length > 0);
    }, [searchQuery]);

    const totalMatches = filteredData.reduce((sum, s) => sum + s.entries.length, 0);

    useEffect(() => {
        if (searchQuery && filteredData.length > 0) {
            if (!filteredData.find((s) => s.id === activeTab)) {
                setActiveTab(filteredData[0].id);
            }
        }
    }, [searchQuery, filteredData, activeTab]);

    return (
        <Card>
            <CardHeader>
                <div className='flex items-center justify-between'>
                    <CardTitle>Admin Guide</CardTitle>
                    <div className='relative w-64'>
                        <Search className='absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground' />
                        <Input
                            placeholder='Search guide...'
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className='pl-8'
                        />
                    </div>
                </div>
                {searchQuery && (
                    <p className='text-sm text-muted-foreground'>
                        {totalMatches} {totalMatches === 1 ? 'result' : 'results'} found
                    </p>
                )}
            </CardHeader>
            <CardContent>
                {filteredData.length > 0 ? (
                    <Tabs value={activeTab} onValueChange={setActiveTab}>
                        <TabsList className='h-auto flex-wrap'>
                            {filteredData.map((section) => (
                                <TabsTrigger key={section.id} value={section.id}>
                                    {section.title}
                                    {searchQuery && (
                                        <Badge variant='secondary' className='ml-1.5'>
                                            {section.entries.length}
                                        </Badge>
                                    )}
                                </TabsTrigger>
                            ))}
                        </TabsList>
                        {filteredData.map((section) => (
                            <TabsContent key={section.id} value={section.id}>
                                <div className='space-y-2'>
                                    <p className='mb-4 text-sm text-muted-foreground'>
                                        {section.description}
                                    </p>
                                    {section.entries.map((entry, i) => (
                                        <GuideEntryCollapsible
                                            key={i}
                                            entry={entry}
                                            defaultOpen={!!searchQuery}
                                        />
                                    ))}
                                </div>
                            </TabsContent>
                        ))}
                    </Tabs>
                ) : (
                    <div className='py-12 text-center text-muted-foreground'>
                        No guide sections match &ldquo;{searchQuery}&rdquo;
                    </div>
                )}
            </CardContent>
        </Card>
    );
};
