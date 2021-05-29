import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Path, PlaceDetails, PlacesService } from 'src/app/api/places.service';

@Component({
    selector: 'app-details-place',
    templateUrl: './details-place.component.html',
    styleUrls: ['./details-place.component.css']
})
export class DetailsPlaceComponent implements OnInit {

    id: string;
    placeDetails: PlaceDetails;

    constructor(private route: ActivatedRoute, private placesService: PlacesService) { }

    ngOnInit() {
        this.route.params.subscribe(params => {
            this.id = params.id;
            this.placesService.GetPlace(params.id).subscribe(
                data => {
                    this.placeDetails = data;
                }
            );
        });
    }

    deletePath(path: Path) {
        this.placesService.DeletePath(path.pathId).subscribe(
            data => {
                this.placeDetails.paths = this.placeDetails.paths.filter(_ => _.pathId !== path.pathId);
            }
        );
    }
}
