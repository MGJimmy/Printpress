import { Component, OnInit } from '@angular/core';
import { ReportViewerService } from '../../services/report-viewer.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-report-viewer',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
  ],
  templateUrl: './report-viewer.component.html',
  styleUrl: './report-viewer.component.css'
})
export class ReportViewerComponent implements OnInit {
  pdfUrl: SafeResourceUrl | null = null;
  id: number = 0;

  constructor(
    private reportViewerService: ReportViewerService, 
    private sanitizer: DomSanitizer,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {

      const queryParams = { ...params };
      this.loadReport(queryParams);
    });
  }

  private loadReport(queryParams: { [key: string]: any}): void {
    this.reportViewerService.getReport(queryParams).subscribe((blob: BlobPart) => {
      const pdfBlob = new Blob([blob], { type: 'application/pdf' });
      const pdfUrl = URL.createObjectURL(pdfBlob);
      this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(pdfUrl);
    });
  }
}
