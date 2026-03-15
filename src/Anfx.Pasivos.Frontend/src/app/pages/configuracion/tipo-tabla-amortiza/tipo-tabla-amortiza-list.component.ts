import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConfiguracionesService } from '../../../../api/services/configuraciones.service';
import { TipoTablaAmortizaListItemDto } from '../../../../api/models/tipoTablaAmortizaListItemDto';

@Component({
  selector: 'app-tipo-tabla-amortiza-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './tipo-tabla-amortiza-list.component.html'
})
export class TipoTablaAmortizaListComponent implements OnInit {
  private service = inject(ConfiguracionesService);

  items = signal<TipoTablaAmortizaListItemDto[]>([]);
  totalCount = signal(0);
  currentPage = signal(1);
  pageSize = signal(10);
  isLoading = signal(false);

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
    const q = this.searchControl.value || undefined;

    this.service.apiConfiguracionesTipoTablaAmortizaGet(q, this.currentPage(), this.pageSize()).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.items.set(res.data.results || []);
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
}
