import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Sort } from '@angular/material/sort';
import { Comparer } from '../../../../../helpers/comparer';
import { Product, Vendor } from '../../../../../models/diagram-vulnerabilities.model';

@Component({
  selector: 'app-diagram-software-dialog',
  templateUrl: './diagram-software-dialog.component.html',
  styleUrls: ['./diagram-software-dialog.component.scss']
})
export class DiagramSoftwareDialogComponent implements OnInit {

  product: Product;
  vendor: Vendor;
  componentLabel: string;
  assetType: string;

  comparer: Comparer = new Comparer();

  constructor(private dialog: MatDialogRef<DiagramSoftwareDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.product = data.product;
    this.vendor = data.vendor;
    this.componentLabel = data.componentLabel;
    this.assetType = data.assetType;
  }

  ngOnInit(): void {}

  close() {
    this.dialog.close();
  }

  sortData(sort: Sort) {
    if (!sort.active || sort.direction === "") {
      return;
    }
  }
}
