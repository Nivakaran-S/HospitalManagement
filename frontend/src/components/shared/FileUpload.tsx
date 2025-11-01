'use client';

import React, { useRef } from 'react';
import { Button } from '../ui/button';
import { Upload, X } from 'lucide-react';

interface FileUploadProps {
  label?: string;
  accept?: string;
  multiple?: boolean;
  onUpload: (files: File[]) => void;
  maxSize?: number;
}

export const FileUpload: React.FC<FileUploadProps> = ({
  label = 'Upload File',
  accept = '*',
  multiple = false,
  onUpload,
  maxSize = 5 * 1024 * 1024, // 5MB default
}) => {
  const inputRef = useRef<HTMLInputElement>(null);
  const [files, setFiles] = React.useState<File[]>([]);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFiles = Array.from(e.target.files || []);
    
    const validFiles = selectedFiles.filter(file => {
      if (file.size > maxSize) {
        alert(`${file.name} is too large. Maximum size is ${maxSize / 1024 / 1024}MB`);
        return false;
      }
      return true;
    });

    setFiles(validFiles);
    onUpload(validFiles);
  };

  const removeFile = (index: number) => {
    const newFiles = files.filter((_, i) => i !== index);
    setFiles(newFiles);
    onUpload(newFiles);
  };

  return (
    <div className="space-y-2">
      <input
        ref={inputRef}
        type="file"
        accept={accept}
        multiple={multiple}
        onChange={handleFileChange}
        className="hidden"
      />
      
      <Button
        type="button"
        variant="outline"
        onClick={() => inputRef.current?.click()}
      >
        <Upload size={16} className="mr-2" />
        {label}
      </Button>

      {files.length > 0 && (
        <div className="space-y-2">
          {files.map((file, index) => (
            <div
              key={index}
              className="flex items-center justify-between p-2 bg-gray-50 rounded-lg"
            >
              <span className="text-sm text-gray-700">{file.name}</span>
              <button
                type="button"
                onClick={() => removeFile(index)}
                className="text-red-600 hover:text-red-800"
              >
                <X size={16} />
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};