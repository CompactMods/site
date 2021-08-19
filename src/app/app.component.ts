import { Component } from '@angular/core';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { faGithub } from "@fortawesome/free-brands-svg-icons";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'cm-site';
  faGithub = faGithub;

  constructor(icons: MatIconRegistry, san: DomSanitizer) {
    icons.addSvgIcon("curseforge", san.bypassSecurityTrustResourceUrl("/assets/curseforge.svg"));
  }
}
