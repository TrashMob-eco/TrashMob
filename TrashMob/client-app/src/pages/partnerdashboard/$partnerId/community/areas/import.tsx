import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
    ArrowLeft,
    Upload,
    FileUp,
    Loader2,
    CheckCircle2,
    AlertTriangle,
    XCircle,
    ArrowRight,
    RotateCcw,
} from 'lucide-react';
import { useMap } from '@vis.gl/react-google-maps';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Switch } from '@/components/ui/switch';
import { Badge } from '@/components/ui/badge';
import { Progress } from '@/components/ui/progress';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { useToast } from '@/hooks/use-toast';
import { GoogleMapWithKey as GoogleMap } from '@/components/Map/GoogleMap';
import AdoptableAreaData, { AdoptableAreaType } from '@/components/Models/AdoptableAreaData';
import {
    ParseImportFile,
    ParseImportFile_Response,
    BulkImportAreas,
    BulkImportAreas_Response,
    GetAdoptableAreas,
} from '@/services/adoptable-areas';
import { parseGeoJson, polygonCoordsToPath, lineStringCoordsToPath } from '@/lib/geojson';

// ============================================================================
// Types
// ============================================================================

type WizardStep = 'upload' | 'mapping' | 'importing' | 'results';

type TrashMobField =
    | 'name'
    | 'description'
    | 'areaType'
    | 'cleanupFrequencyDays'
    | 'minEventsPerYear'
    | 'safetyRequirements'
    | 'allowCoAdoption'
    | 'skip';

type FieldMapping = Record<string, TrashMobField>;

const TRASHMOB_FIELD_LABELS: Record<TrashMobField, string> = {
    name: 'Name',
    description: 'Description',
    areaType: 'Area Type',
    cleanupFrequencyDays: 'Cleanup Frequency (days)',
    minEventsPerYear: 'Min Events/Year',
    safetyRequirements: 'Safety Requirements',
    allowCoAdoption: 'Allow Co-Adoption',
    skip: '(Skip)',
};

const VALID_AREA_TYPES = new Set(['Highway', 'Park', 'Trail', 'Waterway', 'Street', 'Spot']);

const ACCEPTED_EXTENSIONS = '.geojson,.json,.kml,.kmz,.zip';

// ============================================================================
// Auto-detection for field mapping
// ============================================================================

const AUTO_DETECT_RULES: Array<{ patterns: string[]; field: TrashMobField }> = [
    { patterns: ['name', 'area_name', 'areaname', 'road_name', 'title', 'label'], field: 'name' },
    { patterns: ['description', 'desc', 'notes', 'comment', 'remarks'], field: 'description' },
    { patterns: ['type', 'area_type', 'areatype', 'category', 'class'], field: 'areaType' },
    { patterns: ['frequency', 'cleanup_frequency', 'cleanupfrequencydays', 'freq'], field: 'cleanupFrequencyDays' },
    { patterns: ['min_events', 'minevents', 'mineventsper', 'minevents_per_year'], field: 'minEventsPerYear' },
    { patterns: ['safety', 'safety_requirements', 'safetyrequirements'], field: 'safetyRequirements' },
    { patterns: ['co_adoption', 'coadoption', 'allowcoadoption', 'allow_co_adoption'], field: 'allowCoAdoption' },
];

function autoDetectMapping(propertyKeys: string[]): FieldMapping {
    const mapping: FieldMapping = {};
    const usedFields = new Set<TrashMobField>();

    for (const key of propertyKeys) {
        const lowerKey = key.toLowerCase().replace(/[-\s]/g, '_');
        for (const rule of AUTO_DETECT_RULES) {
            if (rule.patterns.includes(lowerKey) && !usedFields.has(rule.field)) {
                mapping[key] = rule.field;
                usedFields.add(rule.field);
                break;
            }
        }
        if (!mapping[key]) {
            mapping[key] = 'skip';
        }
    }

    return mapping;
}

// ============================================================================
// Preview Map Component
// ============================================================================

const PREVIEW_MAP_ID = 'importPreviewMap';

const SHAPE_COLORS = ['#10B981', '#3B82F6', '#F59E0B', '#EF4444', '#8B5CF6', '#EC4899'];

function PreviewMapOverlays({ features }: { features: ParseImportFile_Response['features'] }) {
    const map = useMap(PREVIEW_MAP_ID);
    const shapesRef = useRef<Array<google.maps.Polygon | google.maps.Polyline>>([]);

    useEffect(() => {
        if (!map) return;

        // Clear existing shapes
        shapesRef.current.forEach((s) => s.setMap(null));
        shapesRef.current = [];

        const bounds = new google.maps.LatLngBounds();
        let hasValidBounds = false;

        features.forEach((feature, index) => {
            if (!feature.isValid || !feature.geoJson) return;

            const parsed = parseGeoJson(feature.geoJson);
            if (!parsed) return;

            const color = SHAPE_COLORS[index % SHAPE_COLORS.length];

            if (parsed.type === 'Polygon') {
                const path = polygonCoordsToPath(parsed.coordinates);
                if (path.length < 3) return;
                const poly = new google.maps.Polygon({
                    paths: path,
                    map,
                    strokeColor: color,
                    strokeOpacity: 0.9,
                    strokeWeight: 2,
                    fillColor: color,
                    fillOpacity: 0.15,
                    clickable: false,
                });
                shapesRef.current.push(poly);
                path.forEach((p) => {
                    bounds.extend(p);
                    hasValidBounds = true;
                });
            } else if (parsed.type === 'LineString') {
                const path = lineStringCoordsToPath(parsed.coordinates);
                if (path.length < 2) return;
                const line = new google.maps.Polyline({
                    path,
                    map,
                    strokeColor: color,
                    strokeOpacity: 0.9,
                    strokeWeight: 3,
                    clickable: false,
                });
                shapesRef.current.push(line);
                path.forEach((p) => {
                    bounds.extend(p);
                    hasValidBounds = true;
                });
            }
        });

        if (hasValidBounds && !bounds.isEmpty()) {
            map.fitBounds(bounds, { top: 50, right: 50, bottom: 50, left: 50 });
        }

        return () => {
            shapesRef.current.forEach((s) => s.setMap(null));
            shapesRef.current = [];
        };
    }, [map, features]);

    return null;
}

// ============================================================================
// Import Wizard Page
// ============================================================================

export const PartnerCommunityAreasImport = () => {
    const navigate = useNavigate();
    const queryClient = useQueryClient();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };
    const { toast } = useToast();
    const fileInputRef = useRef<HTMLInputElement>(null);

    // Wizard state
    const [step, setStep] = useState<WizardStep>('upload');
    const [parseResult, setParseResult] = useState<ParseImportFile_Response | null>(null);
    const [fieldMapping, setFieldMapping] = useState<FieldMapping>({});
    const [importResult, setImportResult] = useState<BulkImportAreas_Response | null>(null);

    // Default values for unmapped fields
    const [defaults, setDefaults] = useState({
        areaType: 'Street' as AdoptableAreaType,
        cleanupFrequencyDays: 90,
        minEventsPerYear: 4,
        safetyRequirements: '',
        allowCoAdoption: false,
    });

    // Parse mutation
    const { mutate: parseFile, isPending: isParsing } = useMutation({
        mutationKey: ParseImportFile().key,
        mutationFn: (formData: FormData) => ParseImportFile().service({ partnerId }, formData),
        onSuccess: (res) => {
            const data = res.data;
            if (data.error) {
                toast({ variant: 'destructive', title: 'Parse Error', description: data.error });
                return;
            }
            setParseResult(data);
            setFieldMapping(autoDetectMapping(data.propertyKeys));
            setStep('mapping');
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Upload Failed', description: 'Failed to parse the file.' });
        },
    });

    // Import mutation
    const { mutate: importAreas, isPending: isImporting } = useMutation({
        mutationKey: BulkImportAreas().key,
        mutationFn: (areas: AdoptableAreaData[]) => BulkImportAreas().service({ partnerId }, areas),
        onSuccess: (res) => {
            const data = res.data;
            setImportResult(data);
            setStep('results');
            queryClient.invalidateQueries({ queryKey: GetAdoptableAreas({ partnerId }).key });
            if (data.createdCount > 0) {
                toast({
                    variant: 'primary',
                    title: 'Import Complete',
                    description: `Created ${data.createdCount} area${data.createdCount !== 1 ? 's' : ''}.`,
                });
            }
        },
        onError: () => {
            toast({ variant: 'destructive', title: 'Import Failed', description: 'Failed to import areas.' });
        },
    });

    // Handle file selection
    const handleFileSelected = useCallback(
        (file: File) => {
            const formData = new FormData();
            formData.append('file', file);
            parseFile(formData);
        },
        [parseFile],
    );

    const handleDrop = useCallback(
        (e: React.DragEvent) => {
            e.preventDefault();
            const file = e.dataTransfer.files[0];
            if (file) handleFileSelected(file);
        },
        [handleFileSelected],
    );

    const handleFileInputChange = useCallback(
        (e: React.ChangeEvent<HTMLInputElement>) => {
            const file = e.target.files?.[0];
            if (file) handleFileSelected(file);
        },
        [handleFileSelected],
    );

    // Build mapped areas from parsed features + field mapping
    const mappedAreas = useMemo(() => {
        if (!parseResult) return [];

        return parseResult.features
            .filter((f) => f.isValid)
            .map((feature) => {
                const area = new AdoptableAreaData();
                area.geoJson = feature.geoJson;
                area.areaType = defaults.areaType;
                area.cleanupFrequencyDays = defaults.cleanupFrequencyDays;
                area.minEventsPerYear = defaults.minEventsPerYear;
                area.safetyRequirements = defaults.safetyRequirements;
                area.allowCoAdoption = defaults.allowCoAdoption;

                // Apply field mapping
                for (const [sourceKey, targetField] of Object.entries(fieldMapping)) {
                    if (targetField === 'skip') continue;
                    const val = feature.properties[sourceKey];
                    if (val === undefined || val === '') continue;

                    switch (targetField) {
                        case 'name':
                            area.name = val;
                            break;
                        case 'description':
                            area.description = val;
                            break;
                        case 'areaType':
                            if (VALID_AREA_TYPES.has(val)) area.areaType = val as AdoptableAreaType;
                            break;
                        case 'cleanupFrequencyDays': {
                            const n = parseInt(val, 10);
                            if (!isNaN(n) && n >= 1 && n <= 365) area.cleanupFrequencyDays = n;
                            break;
                        }
                        case 'minEventsPerYear': {
                            const n = parseInt(val, 10);
                            if (!isNaN(n) && n >= 1 && n <= 52) area.minEventsPerYear = n;
                            break;
                        }
                        case 'safetyRequirements':
                            area.safetyRequirements = val;
                            break;
                        case 'allowCoAdoption':
                            area.allowCoAdoption = val === 'true' || val === '1' || val === 'yes';
                            break;
                    }
                }

                return area;
            });
    }, [parseResult, fieldMapping, defaults]);

    // Validation
    const validationIssues = useMemo(() => {
        const issues: Array<{ index: number; name: string; message: string }> = [];
        mappedAreas.forEach((area, i) => {
            if (!area.name || area.name.trim() === '') {
                issues.push({ index: i, name: `Feature ${i + 1}`, message: 'Name is required' });
            }
        });
        return issues;
    }, [mappedAreas]);

    const areasReadyToImport = mappedAreas.filter((a) => a.name && a.name.trim() !== '');

    const handleImport = useCallback(() => {
        if (areasReadyToImport.length === 0) return;
        setStep('importing');
        importAreas(areasReadyToImport);
    }, [areasReadyToImport, importAreas]);

    const handleReset = useCallback(() => {
        setStep('upload');
        setParseResult(null);
        setFieldMapping({});
        setImportResult(null);
        if (fileInputRef.current) fileInputRef.current.value = '';
    }, []);

    return (
        <div className='py-8 space-y-6'>
            {/* Header */}
            <div className='flex items-center gap-4'>
                <Button variant='ghost' size='sm' asChild>
                    <Link to={`/partnerdashboard/${partnerId}/community/areas`}>
                        <ArrowLeft className='h-4 w-4 mr-2' />
                        Back to Areas
                    </Link>
                </Button>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <FileUp className='h-5 w-5' />
                        Import Areas
                    </CardTitle>
                    <CardDescription>
                        Upload a GeoJSON, KML, KMZ, or Shapefile (.zip) to bulk import adoptable areas.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    {/* Step indicator */}
                    <div className='flex items-center gap-2 mb-6 text-sm'>
                        <StepIndicator label='Upload' active={step === 'upload'} done={step !== 'upload'} />
                        <ArrowRight className='h-4 w-4 text-muted-foreground' />
                        <StepIndicator
                            label='Map & Preview'
                            active={step === 'mapping'}
                            done={step === 'importing' || step === 'results'}
                        />
                        <ArrowRight className='h-4 w-4 text-muted-foreground' />
                        <StepIndicator label='Results' active={step === 'results'} done={false} />
                    </div>

                    {/* Step content */}
                    {step === 'upload' ? (
                        <UploadStep
                            isParsing={isParsing}
                            fileInputRef={fileInputRef}
                            onDrop={handleDrop}
                            onFileInputChange={handleFileInputChange}
                        />
                    ) : null}

                    {step === 'mapping' && parseResult ? (
                        <MappingStep
                            parseResult={parseResult}
                            fieldMapping={fieldMapping}
                            onFieldMappingChange={setFieldMapping}
                            defaults={defaults}
                            onDefaultsChange={setDefaults}
                            mappedAreas={mappedAreas}
                            validationIssues={validationIssues}
                            areasReadyCount={areasReadyToImport.length}
                            onImport={handleImport}
                            onBack={handleReset}
                        />
                    ) : null}

                    {step === 'importing' ? (
                        <div className='flex flex-col items-center gap-4 py-12'>
                            <Loader2 className='h-8 w-8 animate-spin' />
                            <p className='text-muted-foreground'>Importing {areasReadyToImport.length} areas...</p>
                            <Progress value={isImporting ? 50 : 100} className='w-64' />
                        </div>
                    ) : null}

                    {step === 'results' && importResult ? (
                        <ResultsStep
                            result={importResult}
                            partnerId={partnerId}
                            onImportMore={handleReset}
                            onViewAreas={() => navigate(`/partnerdashboard/${partnerId}/community/areas`)}
                        />
                    ) : null}
                </CardContent>
            </Card>
        </div>
    );
};

// ============================================================================
// Sub-components
// ============================================================================

function StepIndicator({ label, active, done }: { label: string; active: boolean; done: boolean }) {
    return (
        <span
            className={`px-3 py-1 rounded-full text-xs font-medium ${active ? 'bg-primary text-primary-foreground' : done ? 'bg-green-100 text-green-800' : 'bg-muted text-muted-foreground'}`}
        >
            {done ? <CheckCircle2 className='h-3 w-3 inline mr-1' /> : null}
            {label}
        </span>
    );
}

function UploadStep({
    isParsing,
    fileInputRef,
    onDrop,
    onFileInputChange,
}: {
    isParsing: boolean;
    fileInputRef: React.RefObject<HTMLInputElement | null>;
    onDrop: (e: React.DragEvent) => void;
    onFileInputChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}) {
    return (
        <div
            className='border-2 border-dashed rounded-lg p-12 text-center hover:border-primary/50 transition-colors'
            onDragOver={(e) => e.preventDefault()}
            onDrop={onDrop}
        >
            {isParsing ? (
                <div className='flex flex-col items-center gap-3'>
                    <Loader2 className='h-10 w-10 animate-spin text-primary' />
                    <p className='text-muted-foreground'>Parsing file...</p>
                </div>
            ) : (
                <>
                    <Upload className='h-10 w-10 mx-auto text-muted-foreground mb-4' />
                    <h3 className='text-lg font-medium mb-2'>Drag and drop your file here</h3>
                    <p className='text-muted-foreground mb-4'>
                        Supported formats: GeoJSON (.geojson, .json), KML (.kml), KMZ (.kmz), Shapefile (.zip)
                    </p>
                    <p className='text-muted-foreground text-sm mb-4'>Maximum 500 features, 10MB file size</p>
                    <Button variant='outline' onClick={() => fileInputRef.current?.click()}>
                        <FileUp className='h-4 w-4 mr-2' />
                        Browse Files
                    </Button>
                    <input
                        ref={fileInputRef}
                        type='file'
                        accept={ACCEPTED_EXTENSIONS}
                        className='hidden'
                        onChange={onFileInputChange}
                    />
                </>
            )}
        </div>
    );
}

function MappingStep({
    parseResult,
    fieldMapping,
    onFieldMappingChange,
    defaults,
    onDefaultsChange,
    mappedAreas,
    validationIssues,
    areasReadyCount,
    onImport,
    onBack,
}: {
    parseResult: ParseImportFile_Response;
    fieldMapping: FieldMapping;
    onFieldMappingChange: (m: FieldMapping) => void;
    defaults: {
        areaType: AdoptableAreaType;
        cleanupFrequencyDays: number;
        minEventsPerYear: number;
        safetyRequirements: string;
        allowCoAdoption: boolean;
    };
    onDefaultsChange: (d: typeof defaults) => void;
    mappedAreas: AdoptableAreaData[];
    validationIssues: Array<{ index: number; name: string; message: string }>;
    areasReadyCount: number;
    onImport: () => void;
    onBack: () => void;
}) {
    const handleFieldChange = useCallback(
        (sourceKey: string, targetField: TrashMobField) => {
            onFieldMappingChange({ ...fieldMapping, [sourceKey]: targetField });
        },
        [fieldMapping, onFieldMappingChange],
    );

    return (
        <div className='space-y-6'>
            {/* Summary */}
            <div className='flex items-center gap-4 text-sm'>
                <Badge variant='outline'>
                    {parseResult.totalFeatures} total feature{parseResult.totalFeatures !== 1 ? 's' : ''}
                </Badge>
                <Badge className='bg-green-100 text-green-800'>
                    {parseResult.validFeatures} valid
                </Badge>
                {parseResult.totalFeatures - parseResult.validFeatures > 0 ? (
                    <Badge className='bg-yellow-100 text-yellow-800'>
                        {parseResult.totalFeatures - parseResult.validFeatures} invalid (skipped)
                    </Badge>
                ) : null}
            </div>

            {/* Warnings */}
            {parseResult.warnings.length > 0 ? (
                <div className='bg-yellow-50 border border-yellow-200 rounded-lg p-3 text-sm'>
                    <div className='flex items-center gap-2 font-medium text-yellow-800 mb-1'>
                        <AlertTriangle className='h-4 w-4' />
                        Warnings
                    </div>
                    <ul className='list-disc list-inside text-yellow-700 space-y-0.5'>
                        {parseResult.warnings.slice(0, 10).map((w, i) => (
                            <li key={i}>{w}</li>
                        ))}
                        {parseResult.warnings.length > 10 ? (
                            <li>...and {parseResult.warnings.length - 10} more</li>
                        ) : null}
                    </ul>
                </div>
            ) : null}

            {/* Preview map */}
            <div>
                <h4 className='text-sm font-medium mb-2'>Preview</h4>
                <div className='h-[300px] rounded-lg overflow-hidden border'>
                    <GoogleMap
                        id={PREVIEW_MAP_ID}
                        gestureHandling='greedy'
                        defaultCenter={{ lat: 47.6062, lng: -122.3321 }}
                        defaultZoom={4}
                        style={{ width: '100%', height: '300px' }}
                    >
                        <PreviewMapOverlays features={parseResult.features} />
                    </GoogleMap>
                </div>
            </div>

            {/* Field mapping */}
            {parseResult.propertyKeys.length > 0 ? (
                <div>
                    <h4 className='text-sm font-medium mb-2'>Field Mapping</h4>
                    <p className='text-xs text-muted-foreground mb-3'>
                        Map source properties to TrashMob fields. Auto-detected mappings are pre-selected.
                    </p>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Source Property</TableHead>
                                <TableHead>TrashMob Field</TableHead>
                                <TableHead>Sample Value</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {parseResult.propertyKeys.map((key) => {
                                const sampleFeature = parseResult.features.find((f) => f.properties[key]);
                                const sampleValue = sampleFeature?.properties[key] ?? '';
                                return (
                                    <TableRow key={key}>
                                        <TableCell className='font-mono text-sm'>{key}</TableCell>
                                        <TableCell>
                                            <Select
                                                value={fieldMapping[key] || 'skip'}
                                                onValueChange={(val) =>
                                                    handleFieldChange(key, val as TrashMobField)
                                                }
                                            >
                                                <SelectTrigger className='w-48'>
                                                    <SelectValue />
                                                </SelectTrigger>
                                                <SelectContent>
                                                    {Object.entries(TRASHMOB_FIELD_LABELS).map(([field, label]) => (
                                                        <SelectItem key={field} value={field}>
                                                            {label}
                                                        </SelectItem>
                                                    ))}
                                                </SelectContent>
                                            </Select>
                                        </TableCell>
                                        <TableCell className='text-sm text-muted-foreground max-w-48 truncate'>
                                            {sampleValue}
                                        </TableCell>
                                    </TableRow>
                                );
                            })}
                        </TableBody>
                    </Table>
                </div>
            ) : null}

            {/* Default values */}
            <div>
                <h4 className='text-sm font-medium mb-2'>Default Values</h4>
                <p className='text-xs text-muted-foreground mb-3'>
                    These defaults apply to fields not mapped from the source file.
                </p>
                <div className='grid grid-cols-2 gap-4'>
                    <div className='space-y-2'>
                        <Label>Area Type</Label>
                        <Select
                            value={defaults.areaType}
                            onValueChange={(val) =>
                                onDefaultsChange({ ...defaults, areaType: val as AdoptableAreaType })
                            }
                        >
                            <SelectTrigger>
                                <SelectValue />
                            </SelectTrigger>
                            <SelectContent>
                                {['Highway', 'Park', 'Trail', 'Waterway', 'Street', 'Spot'].map((t) => (
                                    <SelectItem key={t} value={t}>
                                        {t}
                                    </SelectItem>
                                ))}
                            </SelectContent>
                        </Select>
                    </div>
                    <div className='space-y-2'>
                        <Label>Cleanup Frequency (days)</Label>
                        <Input
                            type='number'
                            min={1}
                            max={365}
                            value={defaults.cleanupFrequencyDays}
                            onChange={(e) =>
                                onDefaultsChange({
                                    ...defaults,
                                    cleanupFrequencyDays: parseInt(e.target.value, 10) || 90,
                                })
                            }
                        />
                    </div>
                    <div className='space-y-2'>
                        <Label>Min Events/Year</Label>
                        <Input
                            type='number'
                            min={1}
                            max={52}
                            value={defaults.minEventsPerYear}
                            onChange={(e) =>
                                onDefaultsChange({
                                    ...defaults,
                                    minEventsPerYear: parseInt(e.target.value, 10) || 4,
                                })
                            }
                        />
                    </div>
                    <div className='flex items-center gap-2 pt-6'>
                        <Switch
                            checked={defaults.allowCoAdoption}
                            onCheckedChange={(val) => onDefaultsChange({ ...defaults, allowCoAdoption: val })}
                        />
                        <Label>Allow Co-Adoption</Label>
                    </div>
                </div>
            </div>

            {/* Validation issues */}
            {validationIssues.length > 0 ? (
                <div className='bg-yellow-50 border border-yellow-200 rounded-lg p-3 text-sm'>
                    <div className='flex items-center gap-2 font-medium text-yellow-800 mb-1'>
                        <AlertTriangle className='h-4 w-4' />
                        {validationIssues.length} feature{validationIssues.length !== 1 ? 's' : ''} missing a name
                        (will be skipped)
                    </div>
                </div>
            ) : null}

            {/* Actions */}
            <div className='flex items-center justify-between pt-4 border-t'>
                <Button variant='outline' onClick={onBack}>
                    <RotateCcw className='h-4 w-4 mr-2' />
                    Start Over
                </Button>
                <Button onClick={onImport} disabled={areasReadyCount === 0}>
                    <Upload className='h-4 w-4 mr-2' />
                    Import {areasReadyCount} Area{areasReadyCount !== 1 ? 's' : ''}
                </Button>
            </div>
        </div>
    );
}

function ResultsStep({
    result,
    partnerId,
    onImportMore,
    onViewAreas,
}: {
    result: BulkImportAreas_Response;
    partnerId: string;
    onImportMore: () => void;
    onViewAreas: () => void;
}) {
    return (
        <div className='space-y-6'>
            <div className='text-center py-4'>
                {result.errorCount === 0 && result.createdCount > 0 ? (
                    <CheckCircle2 className='h-12 w-12 mx-auto text-green-500 mb-3' />
                ) : result.createdCount > 0 ? (
                    <AlertTriangle className='h-12 w-12 mx-auto text-yellow-500 mb-3' />
                ) : (
                    <XCircle className='h-12 w-12 mx-auto text-red-500 mb-3' />
                )}
                <h3 className='text-lg font-medium'>Import Complete</h3>
            </div>

            <div className='grid grid-cols-3 gap-4 text-center'>
                <div className='bg-green-50 rounded-lg p-4'>
                    <div className='text-2xl font-bold text-green-700'>{result.createdCount}</div>
                    <div className='text-sm text-green-600'>Created</div>
                </div>
                <div className='bg-yellow-50 rounded-lg p-4'>
                    <div className='text-2xl font-bold text-yellow-700'>{result.skippedDuplicateCount}</div>
                    <div className='text-sm text-yellow-600'>Duplicates Skipped</div>
                </div>
                <div className='bg-red-50 rounded-lg p-4'>
                    <div className='text-2xl font-bold text-red-700'>{result.errorCount}</div>
                    <div className='text-sm text-red-600'>Errors</div>
                </div>
            </div>

            {result.errors.length > 0 ? (
                <div>
                    <h4 className='text-sm font-medium mb-2'>Error Details</h4>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>#</TableHead>
                                <TableHead>Name</TableHead>
                                <TableHead>Error</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {result.errors.map((err, i) => (
                                <TableRow key={i}>
                                    <TableCell>{err.featureIndex + 1}</TableCell>
                                    <TableCell>{err.featureName}</TableCell>
                                    <TableCell className='text-red-600'>{err.message}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </div>
            ) : null}

            <div className='flex items-center justify-center gap-4 pt-4'>
                <Button variant='outline' onClick={onImportMore}>
                    <Upload className='h-4 w-4 mr-2' />
                    Import More
                </Button>
                <Button onClick={onViewAreas}>View Areas</Button>
            </div>
        </div>
    );
}

export default PartnerCommunityAreasImport;
