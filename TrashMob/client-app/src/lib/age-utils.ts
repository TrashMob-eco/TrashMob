/**
 * Calculate age in years from a date of birth.
 * Uses date-only comparison (ignores time) to match how birthdays work.
 */
export function calculateAge(dob: Date): number {
    const today = new Date();
    let age = today.getFullYear() - dob.getFullYear();
    const monthDiff = today.getMonth() - dob.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < dob.getDate())) {
        age--;
    }
    return age;
}

export function isUnder13(dob: Date): boolean {
    return calculateAge(dob) < 13;
}

export function isMinor(dob: Date): boolean {
    return calculateAge(dob) < 18;
}
