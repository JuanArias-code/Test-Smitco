import { Component, Input, OnChanges, OnDestroy } from '@angular/core';
import { EMPTY, ReplaySubject, switchMap, takeUntil } from 'rxjs';
import { Movie } from 'src/app/models/movie';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { SubscriptionService } from 'src/app/services/subscription.service';
import { WatchlistService } from 'src/app/services/watchlist.service';

@Component({
  selector: 'app-add-to-watchlist',
  templateUrl: './add-to-watchlist.component.html',
  styleUrls: ['./add-to-watchlist.component.scss'],
})
export class AddToWatchlistComponent implements OnChanges, OnDestroy {
  @Input()
  movieId = 0;

  toggle = false;
  buttonText = '';
  fontIcon = 'add_circle';
  private destroyed$ = new ReplaySubject<void>(1);

  constructor(
    private readonly watchlistService: WatchlistService,
    private readonly subscriptionService: SubscriptionService,
    private readonly snackBarService: SnackbarService
  ) {}

  ngOnChanges() {
    this.subscriptionService.watchlistItem$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((movieData: Movie[]) => {
        this.setFavourite(movieData);
        this.setButtonText();
      });
  }

  toggleValue() {
    this.toggle = !this.toggle;
    this.setButtonText();

    this.subscriptionService.userData$
      .asObservable()
      .pipe(
        switchMap((user) => {
          const userId = user.userId;
          if (userId > 0) {
            return this.watchlistService.toggleWatchlistItem(
              userId,
              this.movieId
            );
          } else {
            return EMPTY;
          }
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe({
        next: () => {
          if (this.toggle) {
            this.snackBarService.showSnackBar('Pelicula añadida a tu lista de seguimiento');
          } else {
            this.snackBarService.showSnackBar(
              'Pelicula removida de lista de seguimiento'
            );
          }
        },
        error: (error) => {
          console.error('Se ha producido un error al configurar la lista de seguimiento : ', error);
        },
      });
  }

  private setFavourite(movieData: Movie[]) {
    const favouriteMovie = movieData.find((f) => f.movieId === this.movieId);

    if (favouriteMovie) {
      this.toggle = true;
    } else {
      this.toggle = false;
    }
  }

  private setButtonText() {
    if (this.toggle) {
      this.buttonText = 'Eliminar';
      this.fontIcon = 'remove_circle';
    } else {
      this.buttonText = 'Agregar';
      this.fontIcon = 'add_circle';
    }
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
