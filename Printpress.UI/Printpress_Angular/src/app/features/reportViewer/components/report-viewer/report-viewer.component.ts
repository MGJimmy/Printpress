import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ReportViewerService, ReportType } from '../../services/report-viewer.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { TableColDefinitionModel } from '../../../../shared/models/table-col-definition.model';
import { TableTemplateComponent } from '../../../../shared/components/table-template/table-template.component';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-report-viewer',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    TableTemplateComponent
  ],
  templateUrl: './report-viewer.component.html',
  styleUrl: './report-viewer.component.css'
})
export class ReportViewerComponent implements OnInit {
  pdfUrl: SafeResourceUrl | null = null;
  reportType: ReportType = ReportType.Invoice;
  id: number = 0;

  constructor(
    private reportViewerService: ReportViewerService, 
    private sanitizer: DomSanitizer,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.reportType = params['type'] as ReportType || ReportType.Invoice;
      this.id = Number(params['id']) || 0;
      
      if (this.id) {
        this.loadReport();
      }
    });
  }

  private loadReport(): void {
    this.reportViewerService.getReport(this.reportType, this.id).subscribe((blob: BlobPart) => {
      const pdfBlob = new Blob([blob], { type: 'application/pdf' });
      const pdfUrl = URL.createObjectURL(pdfBlob);
      this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(pdfUrl);
    });
  }
}
