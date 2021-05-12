import { Component, OnInit } from '@angular/core';
import { PlacesService } from '../api/places.service';

export interface TrackFinderRequest {
    from: string;
    to: string;
}

@Component({
    selector: 'app-track-finder',
    templateUrl: './track-finder.component.html',
    styleUrls: ['./track-finder.component.css']
})
export class TrackFinderComponent implements OnInit {

    request: TrackFinderRequest = { from: '', to: '' };
    availablePlacesNames: string[];

    constructor(private placesService: PlacesService) {
    }

    get placesFrom(): string[] {
        return this.availablePlacesNames.filter(_ => _ !== this.request.to);
    }
    get placesTo(): string[] {
        return this.availablePlacesNames.filter(_ => _ !== this.request.from);
    }

    ngOnInit() {
        this.placesService.GetAllPlaces().subscribe(_ => this.availablePlacesNames = _.map(_ => _.name));
    }

    FindTrack() {
        this.placesService.FindTrack(this.request).subscribe(_ => console.log(_));
    }

}
