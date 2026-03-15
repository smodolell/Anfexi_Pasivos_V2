import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfiguracionesService } from '../../../../api/services/configuraciones.service';
import { FondeadorListItemDto } from '../../../../api/models/fondeadorListItemDto';

@Component({
  selector: 'app-fondeador-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './fondeador-list.component.html'
})
export class FondeadorListComponent implements OnInit {
  private service = inject(ConfiguracionesService);

  fondeadores = signal<FondeadorListItemDto[]>([]);
  totalCount = signal(0);
  currentPage = signal(1);
  pageSize = signal(10);
  isLoading = signal(false);
  deleteError = signal<string | null>(null);

  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()));

  searchControl = new FormControl('');

  ngOnInit(): void {
    this.load();

    this.searchControl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage.set(1);
      this.load();
    });
  }

  load(): void {
    this.isLoading.set(true);
    this.deleteError.set(null);
    const q = this.searchControl.value || undefined;

    this.service.getPaginatedFondeadores(q, this.currentPage(), this.pageSize()).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.fondeadores.set(res.data.results || []);
          this.totalCount.set(res.data.totalCount || 0);
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.load();
  }

  delete(item: FondeadorListItemDto): void {
    if (!confirm(`¿Está seguro de eliminar el fondeador "${item.titulo}"?`)) return;

    this.service.deleteFondeador(item.id!).subscribe({
      next: () => this.load(),
      error: (err) => {
        const msg = err?.error?.message || 'Error al eliminar el fondeador.';
        this.deleteError.set(msg);
      }
    });
  }
}
