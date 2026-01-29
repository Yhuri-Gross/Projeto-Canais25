locals {
  name_prefix = "canais25-${var.environment}"

  tags = {
    Project     = "canais25"
    Environment = var.environment
    ManagedBy   = "terraform"
  }

  is_prod = var.environment == "prod"
}


