import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class UtilsService {
  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private toastr: ToastrService,
  ) {}

  // ── Notificaciones ────────────────────────────────────────
  showNotification(title: string, message: string, type: 'success' | 'error' | 'warning' | 'info' = 'info') {
    switch (type) {
      case 'success': this.toastr.success(message, title); break;
      case 'error':   this.toastr.error(message, title);   break;
      case 'warning': this.toastr.warning(message, title); break;
      default:        this.toastr.info(message, title);    break;
    }
  }

  // ── Preloader ─────────────────────────────────────────────
  showPreloader() {
    if (!isPlatformBrowser(this.platformId)) return;
    const el = document.querySelector<HTMLElement>('.preloader');
    if (el) { el.style.opacity = '1'; el.style.display = 'block'; }
  }

  hidePreloader() {
    if (!isPlatformBrowser(this.platformId)) return;
    const el = document.querySelector<HTMLElement>('.preloader');
    if (el) {
      el.style.opacity = '0';
      setTimeout(() => { el.style.display = 'none'; }, 500);
    }
  }

  // ── Body classes ──────────────────────────────────────────
  addBodyClass(className: string) {
    if (isPlatformBrowser(this.platformId)) document.body.classList.add(className);
  }

  removeBodyClass(className: string) {
    if (isPlatformBrowser(this.platformId)) document.body.classList.remove(className);
  }

  // ── Storage ───────────────────────────────────────────────
  setLocalStorage(key: string, value: unknown): void {
    if (!isPlatformBrowser(this.platformId)) return;
    try { localStorage.setItem(key, JSON.stringify(value)); }
    catch (e) { console.error('Error saving to localStorage:', e); }
  }

  getLocalStorage<T>(key: string, defaultValue?: T): T | null {
    if (!isPlatformBrowser(this.platformId)) return defaultValue ?? null;
    try {
      const item = localStorage.getItem(key);
      return item ? JSON.parse(item) : defaultValue ?? null;
    } catch (e) {
      console.error('Error reading from localStorage:', e);
      return defaultValue ?? null;
    }
  }

  removeLocalStorage(key: string): void {
    if (!isPlatformBrowser(this.platformId)) return;
    try { localStorage.removeItem(key); }
    catch (e) { console.error('Error removing from localStorage:', e); }
  }
}
