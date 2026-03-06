import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class UtilsService {
  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  // Notificaciones
  showNotification(title: string, message: string, type: 'success' | 'error' | 'warning' | 'info' = 'info') {
    if (isPlatformBrowser(this.platformId) && (window as any).showNotification) {
      (window as any).showNotification(title, message, type);
    }
  }

  // Preloader
  showPreloader() {
    if (isPlatformBrowser(this.platformId) && (window as any).showPreloader) {
      (window as any).showPreloader();
    }
  }

  hidePreloader() {
    if (isPlatformBrowser(this.platformId) && (window as any).hidePreloader) {
      (window as any).hidePreloader();
    }
  }

  // Body classes
  addBodyClass(className: string) {
    if (isPlatformBrowser(this.platformId) && (window as any).addBodyClass) {
      (window as any).addBodyClass(className);
    }
  }

  removeBodyClass(className: string) {
    if (isPlatformBrowser(this.platformId) && (window as any).removeBodyClass) {
      (window as any).removeBodyClass(className);
    }
  }

  // Storage
  setLocalStorage(key: string, value: any): void {
    if (isPlatformBrowser(this.platformId)) {
      try {
        localStorage.setItem(key, JSON.stringify(value));
      } catch (error) {
        console.error('Error saving to localStorage:', error);
      }
    }
  }

  getLocalStorage<T>(key: string, defaultValue?: T): T | null {
    if (isPlatformBrowser(this.platformId)) {
      try {
        const item = localStorage.getItem(key);
        return item ? JSON.parse(item) : defaultValue || null;
      } catch (error) {
        console.error('Error reading from localStorage:', error);
        return defaultValue || null;
      }
    }
    return defaultValue || null;
  }

  removeLocalStorage(key: string): void {
    if (isPlatformBrowser(this.platformId)) {
      try {
        localStorage.removeItem(key);
      } catch (error) {
        console.error('Error removing from localStorage:', error);
      }
    }
  }
}
