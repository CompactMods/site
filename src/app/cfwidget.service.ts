import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CfWidgetResponse } from './data/cfwidget';

@Injectable({
  providedIn: 'root'
})
export class CfWidgetService {

  constructor(private http: HttpClient) { }

  fetchData(id: string) {
    let opts = {
      withCredentials: false,
      headers: new HttpHeaders({
        // "Access-Control-Allow-Origin": "*"
      })
    };

    return this.http.get<CfWidgetResponse>(`https://api.cfwidget.com/${id}`, opts);
  }
}
