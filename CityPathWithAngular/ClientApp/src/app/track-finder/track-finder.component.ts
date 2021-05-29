import { Component, OnInit } from '@angular/core';
import { PlacesService } from '../api/places.service';

export interface TrackFinderRequest {
    from: string;
    to: string;
}
export interface TrackFinderResponse {
    costs: number[];
    nodeNames: string[];
    totalCost: number;
}
export interface TrackFinderRow {
    index: number;
    from: string;
    to: string;
    cost: number;
}
@Component({
    selector: 'app-track-finder',
    templateUrl: './track-finder.component.html',
    styleUrls: ['./track-finder.component.css']
})
export class TrackFinderComponent implements OnInit {

    request: TrackFinderRequest = { from: '', to: '' };
    availablePlacesNames: string[];
    tracksRows: TrackFinderRow[];
    noPath: boolean;
    totalCost: number;

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
        this.placesService.FindTrack(this.request).subscribe(resp => {
            this.noPath = false;
            if (resp) {
                this.totalCost= resp.totalCost;
                this.tracksRows = [];
                for (let i = 0; i < resp.nodeNames.length - 1; i++) {
                    this.tracksRows.push({
                        index: i + 1,
                        from: resp.nodeNames[i],
                        to: resp.nodeNames[i + 1],
                        cost: resp.costs[i + 1] - resp.costs[i]
                    });
                }
            } else {
                this.noPath = true;
            }

        });
    }

}
