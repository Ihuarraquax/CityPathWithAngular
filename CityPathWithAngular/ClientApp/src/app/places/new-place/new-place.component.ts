import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { PlacesService } from 'src/app/api/places.service';


export interface NewPlaceModel {
    name: string;
    sasiads: { name: string, distance: number }[];
}


@Component({
    selector: 'app-new-place',
    templateUrl: './new-place.component.html',
    styleUrls: ['./new-place.component.css']
})
export class NewPlaceComponent implements OnInit {

    model: NewPlaceModel = {
        name: '',
        sasiads: []
    };
    availablePlacesNames: string[];
    constructor(private placesService: PlacesService, private router: Router) {
    }

    ngOnInit() {
        this.placesService.GetAllPlaces().subscribe(_ => this.availablePlacesNames = _.map(_ => _.name));
    }

    addNewSasiad() {
        this.model.sasiads.push({ name: '', distance: null });
    }

    SavePlace() {
        this.placesService.SavePlace(this.model).subscribe(() => this.router.navigate(['/places']));
    }
}
