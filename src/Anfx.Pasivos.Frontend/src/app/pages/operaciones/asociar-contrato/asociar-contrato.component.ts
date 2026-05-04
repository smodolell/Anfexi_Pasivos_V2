import { Component, ViewChild, ElementRef, computed, effect, inject, signal, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  EMPTY, Subject,
  exhaustMap, switchMap, timeout, catchError, tap, filter,
} from 'rxjs';
import { SelectListsService } from 'src/app/core/api/services/selectLists.service';
import { ContratosService } from 'src/app/core/api/services/contratos.service';
import { SelectItemDto } from 'src/app/core/api/models/selectItemDto';
import { RelActivoPasivoDto } from 'src/app/core/api/models/relActivoPasivoDto';
import { wasHandledByInterceptor } from '../../../interceptors/auth.interceptor';
import { UtilsService } from '@services/utils.service';
import { CardInfoComponent } from '@shared/components/card/card-info.component';

type RelItem = RelActivoPasivoDto & { seleccionado: boolean };

@Component({
  selector: 'app-asociar-contrato',
  standalone: true,
  imports: [ReactiveFormsModule, DecimalPipe, CardInfoComponent],
  templateUrl: './asociar-contrato.component.html',
})
export class AsociarContratoComponent {
  @ViewChild('selectAllCb') selectAllCb!: ElementRef<HTMLInputElement>;
  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;

  private readonly selectSvc = inject(SelectListsService);
  private readonly contratosSvc = inject(ContratosService);
  private readonly utilsService = inject(UtilsService);
  private readonly fb = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);

  fondeadores = signal<SelectItemDto[]>([]);
  contratosPasivos = signal<SelectItemDto[]>([]);
  loadingFondeadores = signal(false);
  loadingContratos = signal(false);

  form = this.fb.group({
    idFondeador: [null as number | null, Validators.required],
    idContratoPasivo: [null as number | null, Validators.required],
  });

  preparado = signal(false);
  items = signal<RelItem[]>([]);
  loading = signal(false);
  saving = signal(false);
  totalCount = signal(0);
  totalPages = signal(0);
  currentPage = signal(1);
  pageSize = signal(10);

  private q = '';
  private page = 1;

  allSelected = computed(() => this.items().length > 0 && this.items().every((i) => i.seleccionado));
  someSelected = computed(() => this.items().some((i) => i.seleccionado) && !this.allSelected());
  totalSeleccionados = computed(() => this.items().filter((i) => i.seleccionado).length);

  private readonly load$ = new Subject<void>();
  private readonly guardar$ = new Subject<void>();

  constructor() {
    effect(() => {
      const some = this.someSelected();
      if (this.selectAllCb?.nativeElement) {
        this.selectAllCb.nativeElement.indeterminate = some;
      }
    });

    this.wireCargarFondeadores();
    this.wireFondeadorChanges();
    this.wireContratoPasivoChanges();
    this.wireLoad();
    this.wireGuardar();
  }

  toggleAll(): void {
    const val = !this.allSelected();
    this.items.update((list) => list.map((i) => ({ ...i, seleccionado: val })));
  }

  toggleItem(item: RelItem): void {
    this.items.update((list) =>
      list.map((i) => (i.id === item.id ? { ...i, seleccionado: !i.seleccionado } : i)),
    );
  }

  onPreparar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.preparado.set(true);
    this.q = '';
    this.page = 1;
    if (this.searchInput?.nativeElement) this.searchInput.nativeElement.value = '';
    this.load$.next();
  }

  onSearch(value: string): void {
    this.q = value;
    this.page = 1;
    this.load$.next();
  }

  nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.page++;
      this.load$.next();
    }
  }

  prevPage(): void {
    if (this.page > 1) {
      this.page--;
      this.load$.next();
    }
  }

  onGuardar(): void {
    const seleccionados = this.items().filter((i) => i.seleccionado);
    if (!seleccionados.length) return;
    this.guardar$.next();
  }

  private wireCargarFondeadores(): void {
    this.loadingFondeadores.set(true);
    this.selectSvc.getFondeadoresSelectList().pipe(
      timeout(30_000),
      takeUntilDestroyed(this.destroyRef),
      catchError((err: unknown) => {
        this.loadingFondeadores.set(false);
        if (!wasHandledByInterceptor(err)) {
          this.utilsService.showNotification('Error', 'Error al cargar fondeadores', 'error');
        }
        return EMPTY;
      }),
    ).subscribe((res) => {
      this.fondeadores.set(res.data ?? []);
      this.loadingFondeadores.set(false);
    });
  }

  private wireFondeadorChanges(): void {
    this.form.get('idFondeador')!.valueChanges.pipe(
      tap(() => {
        this.form.patchValue({ idContratoPasivo: null }, { emitEvent: false });
        this.contratosPasivos.set([]);
        this.preparado.set(false);
        this.items.set([]);
      }),
      filter((idFondeador): idFondeador is number => !!idFondeador),
      tap(() => this.loadingContratos.set(true)),
      switchMap((idFondeador) =>
        this.selectSvc.getContratosPasivosPorFondeador(idFondeador, true).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            this.loadingContratos.set(false);
            if (!wasHandledByInterceptor(err)) {
              this.utilsService.showNotification('Error', 'Error al cargar contratos pasivos', 'error');
            }
            return EMPTY;
          }),
        ),
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      this.contratosPasivos.set(res.data ?? []);
      this.loadingContratos.set(false);
    });
  }

  private wireContratoPasivoChanges(): void {
    this.form.get('idContratoPasivo')!.valueChanges.pipe(
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(() => {
      this.preparado.set(false);
      this.items.set([]);
    });
  }

  private wireLoad(): void {
    this.load$.pipe(
      tap(() => this.loading.set(true)),
      switchMap(() => {
        const { idFondeador, idContratoPasivo } = this.form.getRawValue();
        if (!idFondeador || !idContratoPasivo) return EMPTY;
        const q = this.q || undefined;
        return this.contratosSvc
          .getRelActivoPasivo(idFondeador, idContratoPasivo, q, this.page, this.pageSize())
          .pipe(
            timeout(30_000),
            catchError((err: unknown) => {
              this.items.set([]);
              this.loading.set(false);
              if (!wasHandledByInterceptor(err)) {
                this.utilsService.showNotification('Error', 'Error de conexión al cargar relaciones', 'error');
              }
              return EMPTY;
            }),
          );
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((res) => {
      if (res.success && res.data) {
        const prevSelected = new Set(this.items().filter((i) => i.seleccionado).map((i) => i.id));
        this.items.set(
          (res.data.results ?? []).map((r: RelActivoPasivoDto) => ({
            ...r,
            seleccionado: prevSelected.has(r.id),
          })),
        );
        this.totalCount.set(res.data.totalCount ?? 0);
        this.currentPage.set(res.data.currentPage ?? this.page);
        this.pageSize.set(res.data.pageSize ?? 10);
        this.totalPages.set(res.data.totalPages ?? 0);
      } else {
        this.items.set([]);
        const msg = res.errors?.[0] ?? res.message ?? 'Error al cargar datos';
        this.utilsService.showNotification('Error', msg, 'error');
      }
      this.loading.set(false);
    });
  }

  private wireGuardar(): void {
    this.guardar$.pipe(
      exhaustMap(() => {
        const seleccionados = this.items().filter((i) => i.seleccionado).map((i) => i.id!);
        const idContratoPasivo = this.form.getRawValue().idContratoPasivo!;
        this.saving.set(true);
        return this.contratosSvc.asignarPasivos(idContratoPasivo, { listaContratos: seleccionados }).pipe(
          timeout(30_000),
          catchError((err: unknown) => {
            this.saving.set(false);
            if (!wasHandledByInterceptor(err)) {
              const e = err as { error?: { message?: string; errors?: string[] } };
              const msg = e.error?.message ?? e.error?.errors?.[0] ?? 'Error al guardar la asignación';
              this.utilsService.showNotification('Error', msg, 'error');
            }
            return EMPTY;
          }),
        );
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(() => {
      this.utilsService.showNotification('Éxito', 'Contratos activos asignados correctamente', 'success');
      this.saving.set(false);
      this.load$.next();
    });
  }
}
