import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IProduct } from './models/product';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'Skinet';
  products: IProduct[];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    const url = 'http://localhost:5000/api/products';
    // const url = 'https://localhost:5001/api/products
    const products = this.http.get(url).subscribe(
      (response: any) => {
        console.log(response);
        this.products = response;
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
