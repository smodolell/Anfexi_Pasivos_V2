import {
  Component, OnInit, OnDestroy,
  inject, signal, ChangeDetectionStrategy, ChangeDetectorRef,
} from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';
import { AuthService, User } from '../../../services/auth.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TopbarComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly cdr         = inject(ChangeDetectorRef);
  private readonly destroy$    = new Subject<void>();

  currentUser: User | null = null;
  showLogoutModal = signal(false);

  ngOnInit(): void {
    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        this.currentUser = user;
        this.cdr.markForCheck();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  logout(): void {
    this.showLogoutModal.set(true);
  }

  confirmLogout(): void {
    this.showLogoutModal.set(false);
    this.authService.logout();
  }

  cancelLogout(): void {
    this.showLogoutModal.set(false);
  }

  onModalKeydown(event: KeyboardEvent): void {
    if (event.key === 'Escape') this.cancelLogout();
  }
}
