import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { NewPlaceModel } from '../places/new-place/new-place.component';

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

    SavePlace(model: NewPlaceModel) {
        return this.http.post(this.baseUrl + 'places/', model);
    }
}

export interface Place {
    id: string;
    name: string;
}