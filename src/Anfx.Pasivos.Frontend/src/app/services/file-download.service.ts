import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FileDownloadService {
  download(blob: Blob, fileName: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  downloadWithTimestamp(blob: Blob, baseName: string, extension = 'xlsx'): void {
    const fecha = new Date().toISOString().slice(0, 19).replace(/:/g, '-');
    this.download(blob, `${baseName}_${fecha}.${extension}`);
  }
}
