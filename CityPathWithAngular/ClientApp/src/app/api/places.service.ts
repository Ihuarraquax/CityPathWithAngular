import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { NewPlaceModel } from '../places/new-place/new-place.component';
import { TrackFinderRequest, TrackFinderResponse } from '../track-finder/track-finder.component';

@Injectable({
    providedIn: 'root'
})
export class PlacesService {



    constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {

    }

    SearchPlaces(search: string) {
        return this.http.get<Place[]>(this.baseUrl + 'places/search', {
            params: {
                q: search
            }
        });
    }
    GetAllPlaces() {
        return this.http.get<Place[]>(this.baseUrl + 'places/');
    }

    GetPlace(id: string) {
        return this.http.get<PlaceDetails>(this.baseUrl + 'places/' + id);
    }

    SavePlace(model: NewPlaceModel) {
        return this.http.post(this.baseUrl + 'places/', model);
    }

    DeletePlace(id: string) {
        return this.http.delete(this.baseUrl + 'places/' + id);
    }

    DeletePath(id: number) {
        return this.http.delete(this.baseUrl + 'places/paths/' + id);
    }
    FindTrack(request: TrackFinderRequest) {
        return this.http.post<TrackFinderResponse>(this.baseUrl + 'places/FindTrack', request);
    }
}

export interface Place {
    id: string;
    name: string;
}
export interface PlaceDetails {
    id: string;
    name: string;
    paths: Path[];
}

export interface Path {
    toName: string;
    fromName: string;
    distance: number;
    pathId: number;
}
