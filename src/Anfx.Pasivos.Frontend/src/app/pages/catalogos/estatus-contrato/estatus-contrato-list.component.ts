import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CatalogosService } from '../../../../api/services/catalogos.service';
import { EstatusContratoListItemDto } from '../../../../api/models/models';

@Component({
  selector: 'app-estatus-contrato-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './estatus-contrato-list.component.html'
})
export class EstatusContratoListComponent implements OnInit {
  estatus = signal<EstatusContratoListItemDto[]>([]);
  totalCount = signal(0);
  currentPage = signal(1);
  pageSize = signal(10);
  isLoading = signal(false);

  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()));
  searchControl = new FormControl('');

  constructor(private catalogosService: CatalogosService) {}

  ngOnInit(): void {
    this.loadEstatus();
    this.searchControl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage.set(1);
      this.loadEstatus();
    });
  }

  loadEstatus(): void {
    this.isLoading.set(true);
    const q = this.searchControl.value || '';

    this.catalogosService.apiCatalogosEstatusContratoGet(q, this.currentPage(), this.pageSize()).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.estatus.set(res.data.results || []);
          this.totalCount.set(res.data.totalCount || 0);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadEstatus();
  }
}
