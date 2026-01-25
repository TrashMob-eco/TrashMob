import * as React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Card } from "@/components/ui/card";
import { Camera, Image as ImageIcon, X, MapPin } from "lucide-react";

type PhotoItem = {
  id: string;
  file: File;
  url: string; // object URL for preview
};

const MAX_PHOTOS = 3;

function uid() {
  return typeof crypto !== "undefined" && "randomUUID" in crypto
    ? crypto.randomUUID()
    : `${Date.now()}-${Math.random().toString(16).slice(2)}`;
}

function formatBytes(bytes: number) {
  if (bytes < 1024) return `${bytes} B`;
  const kb = bytes / 1024;
  if (kb < 1024) return `${kb.toFixed(1)} KB`;
  return `${(kb / 1024).toFixed(1)} MB`;
}

export default function CreateLitterReportPage() {
  const cameraRef = React.useRef<HTMLInputElement>(null);
  const galleryRef = React.useRef<HTMLInputElement>(null);

  const [photos, setPhotos] = React.useState<PhotoItem[]>([]);
  const [location, setLocation] = React.useState("Lumphini Park – Zone A"); // demo
  const [title, setTitle] = React.useState("");
  const [description, setDescription] = React.useState("");
  const [isPosting, setIsPosting] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);

  // cleanup object URLs on unmount
  React.useEffect(() => {
    return () => {
      for (const p of photos) URL.revokeObjectURL(p.url);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const addFiles = React.useCallback((files: FileList | null) => {
    setError(null);
    if (!files || files.length === 0) return;

    const incoming = Array.from(files).filter((f) => f.type.startsWith("image/"));
    if (incoming.length === 0) {
      setError("Please select an image file.");
      return;
    }

    setPhotos((prev) => {
      const remaining = MAX_PHOTOS - prev.length;
      if (remaining <= 0) return prev;

      const sliced = incoming.slice(0, remaining);
      const next: PhotoItem[] = sliced.map((file) => ({
        id: uid(),
        file,
        url: URL.createObjectURL(file),
      }));

      return [...prev, ...next];
    });
  }, []);

  const removePhoto = React.useCallback((id: string) => {
    setPhotos((prev) => {
      const target = prev.find((p) => p.id === id);
      if (target) URL.revokeObjectURL(target.url);
      return prev.filter((p) => p.id !== id);
    });
  }, []);

  const canPost =
    photos.length > 0 &&
    title.trim().length >= 3 &&
    description.trim().length >= 10 &&
    !isPosting;

  async function handlePost() {
    setError(null);

    if (!canPost) {
      setError("Please add at least 1 photo, a title, and a short description.");
      return;
    }

    try {
      setIsPosting(true);

      // Example: build multipart form-data (common for photo uploads)
      const form = new FormData();
      form.append("location", location);
      form.append("title", title.trim());
      form.append("description", description.trim());
      photos.forEach((p, i) => form.append(`photos[${i}]`, p.file));

      // TODO: replace with your API call
      // await fetch("/api/litters", { method: "POST", body: form });

      await new Promise((r) => { setTimeout(r, 600); }); // demo delay
      alert("Thank you for your help! Your report has been submitted.");
      // optionally: navigate away / reset
      setPhotos((prev) => {
        prev.forEach((p) => URL.revokeObjectURL(p.url));
        return [];
      });
      setTitle("");
      setDescription("");
    } catch (e: any) {
      setError(e?.message ?? "Something went wrong while posting. Please try again.");
    } finally {
      setIsPosting(false);
    }
  }

  return (
    <div className="min-h-screen bg-background">
      {/* Header */}
      <div className="sticky top-0 z-10 border-b bg-background/80 backdrop-blur">
        <div className="mx-auto flex max-w-md items-center justify-between px-4 py-3">
          <h1 className="text-lg font-semibold">New Litter Report</h1>
          <span className="text-xs text-muted-foreground">{photos.length}/{MAX_PHOTOS} photos</span>
        </div>
      </div>

      <div className="mx-auto max-w-md px-4 py-4 space-y-4">
        {/* Photo section */}
        <Card className="p-4 space-y-3">
          <div className="space-y-1">
            <div className="text-sm font-medium">Photos</div>
            <p className="text-xs text-muted-foreground">
              Clear photos help volunteers find and clean up the area faster.
            </p>
          </div>

          <div className="grid grid-cols-2 gap-2">
            <Button
              type="button"
              className="w-full gap-2"
              onClick={() => cameraRef.current?.click()}
              disabled={photos.length >= MAX_PHOTOS}
            >
              <Camera className="h-4 w-4" />
              Take a photo
            </Button>

            <Button
              type="button"
              variant="outline"
              className="w-full gap-2"
              onClick={() => galleryRef.current?.click()}
              disabled={photos.length >= MAX_PHOTOS}
            >
              <ImageIcon className="h-4 w-4" />
              Choose
            </Button>

            {/* Hidden inputs */}
            <input
              ref={cameraRef}
              type="file"
              accept="image/*"
              capture="environment"
              className="hidden"
              onChange={(e) => {
                addFiles(e.target.files);
                e.currentTarget.value = ""; // allow selecting same photo again
              }}
            />
            <input
              ref={galleryRef}
              type="file"
              accept="image/*"
              className="hidden"
              onChange={(e) => {
                addFiles(e.target.files);
                e.currentTarget.value = "";
              }}
            />
          </div>

          {/* Previews */}
          {photos.length > 0 ? (
            <div className="grid grid-cols-3 gap-2">
              {photos.map((p) => (
                <div key={p.id} className="relative overflow-hidden rounded-lg border">
                  <img
                    src={p.url}
                    alt="Selected"
                    className="h-24 w-full object-cover"
                  />
                  <button
                    type="button"
                    onClick={() => removePhoto(p.id)}
                    className="absolute right-1 top-1 inline-flex h-7 w-7 items-center justify-center rounded-full bg-black/60 text-white hover:bg-black/75"
                    aria-label="Remove photo"
                  >
                    <X className="h-4 w-4" />
                  </button>
                  <div className="px-2 py-1 text-[10px] text-muted-foreground">
                    {formatBytes(p.file.size)}
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="rounded-lg border border-dashed p-3 text-xs text-muted-foreground">
              Tip: Take one wide shot and one close-up if possible.
            </div>
          )}
        </Card>

        {/* Location */}
        <Card className="p-4 space-y-2">
          <Label className="text-sm">Location</Label>
          <div className="flex items-center gap-2 rounded-md border px-3 py-2">
            <MapPin className="h-4 w-4 text-muted-foreground" />
            <div className="text-sm">{location}</div>
          </div>
          <p className="text-xs text-muted-foreground">
            You’ll be able to adjust the pin on the map in the next step (optional).
          </p>
        </Card>

        {/* Title + Description */}
        <Card className="p-4 space-y-4">
          <div className="space-y-2">
            <Label htmlFor="title">Title</Label>
            <Input
              id="title"
              placeholder="e.g., Trash near the jogging path"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              maxLength={80}
            />
            <div className="text-xs text-muted-foreground">
              {title.trim().length}/80
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="desc">Description</Label>
            <Textarea
              id="desc"
              placeholder="Describe what you see and where it is (landmarks help)."
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className="min-h-[110px]"
              maxLength={400}
            />
            <div className="text-xs text-muted-foreground">
              {description.trim().length}/400
            </div>
          </div>

          {error ? (
            <div className="rounded-md border border-destructive/30 bg-destructive/10 px-3 py-2 text-sm text-destructive">
              {error}
            </div>
          ) : null}
        </Card>

        {/* Footer actions */}
        <div className="pb-6 pt-2 flex items-center justify-between">
          <Button type="button" variant="ghost" onClick={() => history.back()}>
            Back
          </Button>

          <Button type="button" onClick={handlePost} disabled={!canPost}>
            {isPosting ? "Posting…" : "Post"}
          </Button>
        </div>

        <p className="text-center text-xs text-muted-foreground pb-6">
          Photos are used to coordinate cleanup and are not shared publicly without consent.
        </p>
      </div>
    </div>
  );
}