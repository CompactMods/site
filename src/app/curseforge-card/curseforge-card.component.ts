import { Component, Input, OnInit } from '@angular/core';
import { faGithub } from "@fortawesome/free-brands-svg-icons";
import { CfWidgetService } from '../cfwidget.service';
import { CfWidgetResponse } from '../data/cfwidget';

@Component({
  selector: 'curseforge-card',
  templateUrl: './curseforge-card.component.html',
  styleUrls: ['./curseforge-card.component.scss']
})
export class CurseforgeCardComponent implements OnInit {
  
  @Input()
  public cf: string = "";
  public loaded: boolean = false;
  
  faGithub = faGithub;
  public cfData?: CfWidgetResponse;

  constructor(private cfw: CfWidgetService) {
  }

  ngOnInit(): void {
    this.cfw.fetchData(this.cf).subscribe((data) => {
      this.cfData = data;
      this.loaded = true;
    }, (err) => {
      console.debug(err);
      this.loaded = true;
    });
  }

  public get downloadSvg() {
    return `https://cf.way2muchnoise.eu/${this.cf}.svg`;
  }

  public get availableForSvg() {
    return `https://cf.way2muchnoise.eu/versions/${this.cf}_latest.svg`;
  }

  public get hasCfUrl() {
    return this.cfData?.urls?.curseforge;
  }
}
