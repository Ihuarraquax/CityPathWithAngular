import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { PlacesService } from 'src/app/api/places.service';

@Component({
    selector: 'app-details-place',
    templateUrl: './details-place.component.html',
    styleUrls: ['./details-place.component.css']
})
export class DetailsPlaceComponent implements OnInit {

    id: string;

    constructor(private route: ActivatedRoute, private placesService: PlacesService) { }

    ngOnInit() {
        this.placesService.GetPlace(this.id).subscribe(
            data => {
                console.log(data);
            }
        );
    }

}
