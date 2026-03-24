import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './pagination.component.html'
})
export class PaginationComponent {
  currentPage = input(1);
  totalPages  = input(0);
  totalCount  = input(0);
  pageSize    = input(10);

  prev = output<void>();
  next = output<void>();

  firstItem = computed(() => (this.currentPage() - 1) * this.pageSize() + 1);
  lastItem  = computed(() => Math.min(this.currentPage() * this.pageSize(), this.totalCount()));
}
