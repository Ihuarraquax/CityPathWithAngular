import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

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
}

export interface Place {
    id: string;
    name: string;
    coordinate: GeoCoordinate;
}

export interface GeoCoordinate {
    latitude: number;
    longitude: number;
};