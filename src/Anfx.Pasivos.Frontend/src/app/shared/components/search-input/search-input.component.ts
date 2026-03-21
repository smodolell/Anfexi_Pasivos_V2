import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  inject,
  input,
  output,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-search-input',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormsModule],
  templateUrl: './search-input.component.html',
  styleUrl: './search-input.component.scss',
})
export class SearchInputComponent implements OnInit {
  placeholder = input('Buscar...');
  value       = input('');
  debounceMs  = input(400);
  label       = input('Buscar');

  searched = output<string>();
  cleared  = output<void>();

  protected term    = '';
  protected focused = false;

  private readonly destroyRef = inject(DestroyRef);
  private readonly subject    = new Subject<string>();

  ngOnInit(): void {
    this.term = this.value();
    this.subject
      .pipe(debounceTime(this.debounceMs()), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(v => this.searched.emit(v));
  }

  onInput(): void {
    this.subject.next(this.term);
  }

  onEnter(): void {
    this.searched.emit(this.term);
  }

  clear(): void {
    this.term = '';
    this.subject.next('');
    this.searched.emit('');
    this.cleared.emit();
  }
}
