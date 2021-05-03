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

    constructor(private placesService: PlacesService) { }

    ngOnInit() {
    }

    searchPlaces() {
        this.placesService.SearchPlaces(this.search.value).subscribe(resp => {
            this.places = resp;
        });
    }
}
