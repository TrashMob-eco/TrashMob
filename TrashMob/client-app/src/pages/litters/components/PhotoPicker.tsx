import { Button } from "@/components/ui/button";
import { Image as ImageIcon } from "lucide-react";
import { useRef } from "react";

export function PhotoPicker({
  onChange,
}: {
  onChange: (files: FileList) => void;
}) {
  const cameraRef = useRef<HTMLInputElement>(null);
  const galleryRef = useRef<HTMLInputElement>(null);

  return (
    <div className="flex flex-col gap-6">
      <Button
        type="button"
        variant="outline"
        className="w-full gap-2 h-64 border-dashed"
        onClick={() => galleryRef.current?.click()}
      >
        <ImageIcon className="h-4 w-4" />
        Add Photo
      </Button>

      <input
        ref={cameraRef}
        type="file"
        accept="image/*"
        capture="environment"
        className="hidden"
        onChange={(e) => {
          const files = e.target.files;
          if (files) onChange(files);
          e.currentTarget.value = "";
        }}
      />

      <input
        ref={galleryRef}
        type="file"
        accept="image/*"
        className="hidden"
        multiple
        onChange={(e) => {
          const files = e.target.files;
          if (files) onChange(files);
          e.currentTarget.value = "";
        }}
      />
    </div>
  );
}