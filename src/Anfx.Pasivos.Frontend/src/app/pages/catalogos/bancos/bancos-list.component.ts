import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CatalogosService } from '../../../../api/services/catalogos.service';
import { BancoListItemDto } from '../../../../api/models/models';

@Component({
  selector: 'app-bancos-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './bancos-list.component.html'
})
export class BancosListComponent implements OnInit {
  // Signals para el estado
  bancos = signal<BancoListItemDto[]>([]);
  totalCount = signal(0);
  currentPage = signal(1);
  pageSize = signal(10);
  isLoading = signal(false);

  // Computado para el total de páginas
  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()));

  searchControl = new FormControl('');

  constructor(private catalogosService: CatalogosService) {}

  ngOnInit(): void {
    this.loadBancos();

    // Filtro con debounce
    this.searchControl.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage.set(1);
      this.loadBancos();
    });
  }

  loadBancos(): void {
    this.isLoading.set(true);
    const q = this.searchControl.value || '';

    this.catalogosService.apiCatalogosBancoGet(q, this.currentPage(), this.pageSize()).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.bancos.set(res.data.results || []);
          this.totalCount.set(res.data.totalCount || 0);
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error cargando bancos:', err);
        this.isLoading.set(false);
      }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadBancos();
  }

  deleteBanco(banco: BancoListItemDto): void {
    if (confirm(`¿Está seguro de eliminar el banco ${banco.banco}?`)) {
      // Nota: El servicio no parece tener un método 'deleteBanco' en CatalogosService.
      // Voy a revisar CatalogosService de nuevo para ver si falta.
      alert('Funcionalidad de borrado no disponible en el servicio de catálogos.');
    }
  }
}
