import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CatalogosService } from '../../../../api/services/catalogos.service';
import { CuentaBancariaListItemDto } from '../../../../api/models/models';

@Component({
  selector: 'app-cuentas-bancarias-list',
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './cuentas-bancarias-list.component.html'
})
export class CuentasBancariasListComponent implements OnInit {
  cuentas = signal<CuentaBancariaListItemDto[]>([]);
  totalCount = signal(0);
  currentPage = signal(1);
  pageSize = signal(10);
  isLoading = signal(false);

  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()));
  searchControl = new FormControl('');

  constructor(private catalogosService: CatalogosService) {}

  ngOnInit(): void {
    this.loadCuentas();
    this.searchControl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage.set(1);
      this.loadCuentas();
    });
  }

  loadCuentas(): void {
    this.isLoading.set(true);
    const q = this.searchControl.value || '';

    this.catalogosService.apiCatalogosCuentaBancariaGet(q, this.currentPage(), this.pageSize()).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.cuentas.set(res.data.results || []);
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
    this.loadCuentas();
  }
}
