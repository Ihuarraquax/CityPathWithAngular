import { Router } from '@angular/router';
import { Place, PlacesService } from './../api/places.service';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
    selector: 'app-places',
    templateUrl: './places.component.html',
    styleUrls: ['./places.component.css']
})
export class PlacesComponent implements OnInit {

    search = new FormControl('');
    places: Place[];

    constructor(
        private placesService: PlacesService,
        private router: Router) { }

    ngOnInit() {
        this.placesService.SearchPlaces('').subscribe(resp => {
            this.places = resp;
        });
    }

    searchPlaces() {
        this.placesService.SearchPlaces(this.search.value).subscribe(resp => {
            this.places = resp;
        });
    }

    deletePlace(place: Place) {
        this.placesService.DeletePlace(place.id).subscribe(resp => {
            this.places = this.places.filter(_ => _.id !== place.id);
        });
    }

    gotoDetails(place: Place) {
        this.router.navigate(['/places/details', { id: place.id }]);
    }
}
