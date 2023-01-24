////////////////////////////////
//
//   Copyright 2023 Battelle Energy Alliance, LLC
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
////////////////////////////////
import { ComponentSoftwareItem } from './../../../../../models/diagram-vulnerabilities.model';
import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Sort } from '@angular/material/sort';
import { Comparer } from '../../../../../helpers/comparer';
import { DiagramService } from './../../../../../services/diagram.service';

@Component({
  selector: 'app-diagram-software-dialog',
  templateUrl: './diagram-software-dialog.component.html',
  styleUrls: ['./diagram-software-dialog.component.scss']
})
export class DiagramSoftwareDialogComponent implements OnInit {

  component: any;
  comparer: Comparer = new Comparer();

  constructor(private diagramSvc: DiagramService,
    private dialog: MatDialogRef<DiagramSoftwareDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.component = data.component
  }

  ngOnInit(): void {}

  close() {
    this.dialog.close();
  }

  addNewSoftwareItem() {
    if (!this.component.softwareItems) {
      this.component.softwareItems = [new ComponentSoftwareItem()];
    } else {
      this.component.softwareItems.push(new ComponentSoftwareItem());
    }
  }

  saveComponent(component) {
    this.diagramSvc.saveComponent(component).subscribe();
  }

  sortData(sort: Sort) {
    if (!sort.active || sort.direction === "") {
      return;
    }

    this.component.softwareItems.sort((a: ComponentSoftwareItem, b: ComponentSoftwareItem) => {
      const isAsc = sort.direction === "asc";
      switch (sort.active) {
        case "vendor":
          return this.comparer.compare(a.vendorName, b.vendorName, isAsc);
        case "name":
          return this.comparer.compare(a.name, b.name, isAsc);
        case "version":
          return this.comparer.compare(a.version, b.version, isAsc);
        default:
          return 0;
      }
    });
  }
}
